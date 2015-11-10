﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft;
using KMBit.Beans;
using KMBit.DAL;
using KMBit.BL.Charge;
using KMBit.Util;
using System.Security.Cryptography;

namespace KMBit.BL.Charge
{
    public class YiRenCharge : ChargeService, ICharge
    {
        private string version = "1.1";

        public YiRenCharge()
        {
            Logger = log4net.LogManager.GetLogger(this.GetType());
        }
        public void CallBack(List<WebRequestParameters> data)
        {
            
        }

        public ChargeResult Charge(ChargeOrder order)
        {
            ChargeResult result = new ChargeResult() { Status= ChargeStatus.FAILED };
            chargebitEntities db = null;
            ProceedOrder(order, out result);
            if (result.Status == ChargeStatus.FAILED)
            {
                return result;
            }
            List<WebRequestParameters> parmeters = new List<WebRequestParameters>();
            bool succeed = false;
            try
            {
                db = new chargebitEntities();
                Charge_Order corder = (from co in db.Charge_Order where co.Id == order.Id select co).FirstOrDefault<Charge_Order>();
                corder.Process_time = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                KMBit.DAL.Resrouce_interface rInterface = (from ri in db.Resrouce_interface where ri.Resource_id == order.ResourceId select ri).FirstOrDefault<Resrouce_interface>();
                Resource_taocan taocan = (from t in db.Resource_taocan where t.Id == order.ResourceTaocanId select t).FirstOrDefault<Resource_taocan>();               
                ServerUri = new Uri(rInterface.APIURL);
                parmeters.Add(new WebRequestParameters("V", version, false));
                parmeters.Add(new WebRequestParameters("Action", "charge", false));
                parmeters.Add(new WebRequestParameters("Range", taocan.Area_id > 0 ? "1" : "0", false));
                SortedDictionary<string, string> paras = new SortedDictionary<string, string>();
                paras["Account"] = rInterface.Username;
                paras["Mobile"] = order.MobileNumber;
                paras["Package"] = taocan.Quantity.ToString();
                string signStr = "";
                foreach (KeyValuePair<string, string> p in paras)
                {
                    if (signStr == string.Empty)
                    {
                        signStr += p.Key.ToLower() + "=" + p.Value;
                    }
                    else
                    {
                        signStr += "&" + p.Key.ToLower() + "=" + p.Value;
                    }
                }
                signStr += "&key=" + KMAes.DecryptStringAES(rInterface.Userpassword);
                paras["Sign"] = GetMD5(signStr);

                foreach (KeyValuePair<string, string> p in paras)
                {
                    parmeters.Add(new WebRequestParameters(p.Key, p.Value, false));
                }
                SendRequest(parmeters, false, out succeed);
                if (!string.IsNullOrEmpty(Response))
                {
                    JObject jsonResult = JObject.Parse(Response);
                    order.OutId = jsonResult["TaskID"] != null ? jsonResult["TaskID"].ToString() : "";
                    string code= jsonResult["Code"] != null ? jsonResult["Code"].ToString() : "";
                    string message= jsonResult["Message"] != null ? jsonResult["Message"].ToString() : "";
                    result.Message = message;
                    switch (code)
                    {
                        case "0":
                            result.Message = ChargeConstant.CHARGING;
                            result.Status = ChargeStatus.ONPROGRESS;
                            break;
                        case "001":
                            result.Status = ChargeStatus.FAILED;
                            break;
                        case "002":
                            result.Status = ChargeStatus.FAILED;
                            break;
                        case "003":
                            result.Status = ChargeStatus.FAILED;
                            break;
                        case "004":
                            result.Status = ChargeStatus.FAILED;
                            result.Message = ChargeConstant.RESOURCE_NOT_ENOUGH_MONEY;
                            break;
                        case "005":
                            result.Status = ChargeStatus.FAILED;
                            break;
                        case "006":
                            result.Status = ChargeStatus.FAILED;
                            break;
                        case "007":
                            result.Status = ChargeStatus.FAILED;
                            break;
                        case "008":
                            result.Status = ChargeStatus.FAILED;
                            break;
                        case "009":
                            result.Status = ChargeStatus.FAILED;
                            break;
                        case "100":
                            result.Status = ChargeStatus.FAILED;
                            break;
                        case "999":
                            result.Status = ChargeStatus.FAILED;
                            break;
                        default:
                            result.Status = ChargeStatus.FAILED;
                            break;
                    }
                    ChangeOrderStatus(order, result);
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }finally
            {
                if(db!=null)
                {
                    db.Dispose();
                }
            }
            return result;
        }

        public void ImportProducts(int resourceId, int operate_user)
        {
            chargebitEntities db = null;
            try
            {
                bool succeed = false;
                List<WebRequestParameters> parmeters = new List<WebRequestParameters>();
                parmeters.Add(new WebRequestParameters("V",version,false));
                parmeters.Add(new WebRequestParameters("Action", "getPackage", false));
                db = new chargebitEntities();
                db.Configuration.AutoDetectChangesEnabled = false;
                KMBit.DAL.Resrouce_interface rInterface = (from ri in db.Resrouce_interface where ri.Resource_id == resourceId select ri).FirstOrDefault<Resrouce_interface>();
                ServerUri = new Uri(rInterface.ProductApiUrl);
                SortedDictionary<string, string> paras = new SortedDictionary<string, string>();
                paras["Account"] = rInterface.Username;
                paras["Type"] = "0";
                string signStr = "";
                foreach(KeyValuePair<string,string> p in paras)
                {
                    if(signStr==string.Empty)
                    {
                        signStr += p.Key.ToLower() + "=" + p.Value;
                    }else
                    {
                        signStr += "&"+p.Key.ToLower() + "=" + p.Value;
                    }                    
                }
                signStr += "&key="+KMAes.DecryptStringAES(rInterface.Userpassword);
                paras["Sign"] = GetMD5(signStr);
                foreach (KeyValuePair<string, string> p in paras)
                {
                    parmeters.Add(new WebRequestParameters(p.Key,p.Value,false));
                }

                SendRequest(parmeters, false, out succeed);
                if(succeed)
                {
                    if(!string.IsNullOrEmpty(Response))
                    {
                        JObject json = JObject.Parse(Response);
                        string code = json["Code"]!=null? json["Code"].ToString():"";
                        string message= json["Message"] != null ? json["Message"].ToString() : "";
                        if(!string.IsNullOrEmpty(code) && code=="0" && !string.IsNullOrEmpty(message) && message=="OK")
                        {
                            JArray packages = (JArray)json["Packages"];
                            if(packages!=null)
                            {
                                for(int i=0;i<packages.Count;i++)
                                {
                                    JObject package = (JObject)packages[i];

                                    if(package!=null)
                                    {
                                        Resource_taocan taocan = null;
                                        int sp = 0;
                                        int spId = 0;
                                        int.TryParse(package["Type"].ToString(),out sp);
                                        int quantity = 0;
                                        int.TryParse(package["Package"].ToString(), out quantity);
                                        float price = 0;
                                        float.TryParse(package["Price"].ToString(), out price);

                                        if (sp == 2)
                                        {
                                            spId = 3;
                                        }
                                        else if (sp == 1)
                                        {
                                            spId = 1;
                                        }
                                        else if (sp ==3)
                                        {
                                            spId = 2;
                                        }

                                        taocan = (from t in db.Resource_taocan where t.Resource_id==rInterface.Resource_id && t.Quantity==quantity && t.Sp_id== spId select t).FirstOrDefault<Resource_taocan>();
                                        if (taocan != null)
                                        {
                                            taocan.Purchase_price = price;
                                            taocan.UpdatedBy = operate_user;
                                            taocan.Updated_time = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            taocan = new Resource_taocan()
                                            {
                                                Area_id = 0,
                                                CreatedBy = operate_user,
                                                Created_time = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                                                Enabled = false,
                                                EnableDiscount = true,
                                                Purchase_price = price,
                                                Quantity = quantity,
                                                Resource_id = rInterface.Resource_id,
                                                Sale_price = price,
                                                Serial = "",
                                                Sp_id = spId,
                                                Taocan_id = 0,
                                                Resource_Discount = 1
                                            };

                                            Taocan ntaocan = (from t in db.Taocan where t.Sp_id == taocan.Sp_id && t.Quantity == taocan.Quantity select t).FirstOrDefault<Taocan>();
                                            Sp spO = (from s in db.Sp where s.Id == taocan.Sp_id select s).FirstOrDefault<Sp>();
                                            if (ntaocan == null)
                                            {
                                                string taocanName = spO != null ? spO.Name + " " + taocan.Quantity.ToString() + "M" : "全网 " + taocan.Quantity.ToString() + "M";
                                                ntaocan = new Taocan() { Created_time = taocan.Created_time, Description = taocanName, Name = taocanName, Sp_id = taocan.Sp_id, Quantity = taocan.Quantity, Updated_time = 0 };
                                                db.Taocan.Add(ntaocan);
                                                db.SaveChanges();
                                            }
                                            if (ntaocan.Id > 0)
                                            {
                                                taocan.Taocan_id = ntaocan.Id;
                                                db.Resource_taocan.Add(taocan);
                                            }
                                        }
                                    }

                                }
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
            finally
            {
                if(db!=null)
                {
                    db.Dispose();
                }
            }
        }

        private string GetMD5(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }
}
