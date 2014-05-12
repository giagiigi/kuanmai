﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using KM.JXC.BL;
using KM.JXC.BL.Models;
using KM.JXC.DBA;
using Newtonsoft.Json.Linq;
using KM.JXC.Web.Models;
using KM.JXC.Common.Util;
namespace KM.JXC.Web.Controllers.api
{
    public class ProductsController : BaseApiController
    {
        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        public ApiMessage UpdateProduct()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            ApiMessage message = new ApiMessage();
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            ProductManager pdtManager = new ProductManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            int product_id = 0;
            int categoryId = 0;
            string description = "";
            string props = "";
            string images = "";
            string title = "";
            int.TryParse(request["cid"], out categoryId);
            int.TryParse(request["product_id"],out product_id);
            description = request["desc"];
            title = request["title"];
            images = request["images"];
            props = request["props"];
            string suppliers = request["sids"];
            try
            {
                BProduct product = new BProduct();
                product.ID = product_id;
                product.Parent = null;
                product.Category = new BCategory() { ID = categoryId };
                product.Title = title;
                product.Description = description;
                product.Properties = null;
                product.FileRootPath = request.PhysicalApplicationPath;
                if (!string.IsNullOrEmpty(images))
                {
                    product.Images = new List<Image>();
                    string[] ims = images.Split(',');
                    foreach (string img in ims)
                    {
                        int image_id = 0;
                        int.TryParse(img,out image_id);
                        if (image_id > 0)
                        {
                            product.Images.Add(new Image() { ID = image_id });
                        }
                    }
                }

                if (!string.IsNullOrEmpty(suppliers))
                {
                    product.Suppliers = new List<Supplier>();
                    string[] sids = suppliers.Split(',');
                    foreach (string sid in sids)
                    {
                        product.Suppliers.Add(new Supplier() { Supplier_ID = int.Parse(sid), Enabled = true });
                    }
                }

                if (!string.IsNullOrEmpty(props))
                {
                    if (product.Children == null)
                    {
                        product.Children = new List<BProduct>();
                    }
                    string[] groups = props.Split(';');
                    foreach (string group in groups)
                    {
                        string groupp = group.Split('|')[1];
                        int pdtId = int.Parse(group.Split('|')[0]);
                        BProduct child = new BProduct();
                        child.ID = pdtId;
                        child.Title = product.Title;
                        child.Description = product.Description;
                        child.Category = product.Category;
                        List<BProductProperty> properties = new List<BProductProperty>();
                        string[] pops = groupp.Split(',');
                        foreach (string pop in pops)
                        {
                            BProductProperty prop = new BProductProperty();
                            prop.PID = int.Parse(pop.Split(':')[0]);
                            prop.PVID = int.Parse(pop.Split(':')[1]);
                            properties.Add(prop);
                        }
                        child.Properties = properties;
                        product.Children.Add(child);
                    }
                }

                pdtManager.UpdateProduct(ref product);
                message.Status = "ok";
                message.Item = product;
            }
            catch (KM.JXC.Common.KMException.KMJXCException kex)
            {
                message.Status = "failed";
                message.Message = kex.Message;
            }
            catch (Exception ex)
            {
                message.Status = "failed";
                message.Message = ex.Message;
            }
            return message;
        }

        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        public ApiMessage Create()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            ApiMessage message = new ApiMessage();
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            ProductManager pdtManager = new ProductManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            int categoryId = 0;
            string description = "";
            string props = "";
            string images = "";
            string title = "";
            string suppliers = "";
            int.TryParse(request["cid"],out categoryId);
            description = request["desc"];
            title=request["title"];
            images = request["images"];
            props=request["props"];
            suppliers = request["sids"];

            try
            {
                BProduct product = new BProduct();
                product.Parent = null;
                product.Category = new BCategory() { ID =categoryId};
                product.Title = title;
                product.Description = description;
                product.Properties = null;
                if (!string.IsNullOrEmpty(images))
                {
                    product.Images = new List<Image>();
                    string[] ims = images.Split(',');
                    foreach (string img in ims) {
                        product.Images.Add(new Image() { ID=int.Parse(img) });
                    }
                }

                if (!string.IsNullOrEmpty(suppliers))
                {
                    product.Suppliers = new List<Supplier>();
                    string[] sids = suppliers.Split(',');
                    foreach (string sid in sids) 
                    {
                        product.Suppliers.Add(new Supplier() { Supplier_ID=int.Parse(sid), Enabled=true});
                    }
                }

                if (!string.IsNullOrEmpty(props))
                {
                    if (product.Children == null)
                    {
                        product.Children = new List<BProduct>();
                    }
                    string[] groups = props.Split(';');
                    foreach (string group in groups)
                    {
                        BProduct child = new BProduct();
                        child.Title = product.Title;
                        child.Description = product.Description;
                        child.Category = product.Category;                        
                        List<BProductProperty> properties = new List<BProductProperty>();
                        string[] pops = group.Split(',');
                        foreach (string pop in pops)
                        {
                            BProductProperty prop = new BProductProperty();
                            prop.PID = int.Parse(pop.Split(':')[0]);
                            prop.PVID = int.Parse(pop.Split(':')[1]);
                            properties.Add(prop);
                        }
                        child.Properties = properties;
                        product.Children.Add(child);
                    }
                }

                pdtManager.CreateProduct(product);
                message.Status = "ok";
                message.Item = product;
            }
            catch (KM.JXC.Common.KMException.KMJXCException kex)
            {
            }
            catch (Exception ex)
            {
            }

            return message;
        }

        [HttpPost]
        public PQGridData SearchProducts()
        {
            PQGridData data = new PQGridData();
            int page = 0;
            int pageSize = 30;
            int total = 0;
            int? category_id = null;
            string keyword = "";
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            ProductManager pdtManager = new ProductManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            int.TryParse(request["page"],out page);
            int.TryParse(request["pageSize"],out pageSize);
            if (request["cid"] != null && request["cid"].ToString() != "" && request["cid"].ToString()!="0")
            {
                int cid = 0;
                int.TryParse(request["cid"],out cid);
                if (cid > 0) {
                    category_id = cid;
                }
            }
            keyword = request["keyword"];
            int[] sids = null;
            string suppliers = request["suppliers"];
            if (!string.IsNullOrEmpty(suppliers))
            {
                string[] ss = suppliers.Split(',');
                sids = new int[ss.Length];
                for (int i = 0; i < ss.Length; i++)
                {
                    sids[i] = int.Parse(ss[i]);
                }
            }
            data.data = pdtManager.SearchProducts(sids, keyword, "", 0, 0, category_id, page, pageSize, out total);
            data.totalRecords = total;
            data.curPage = page;
            return data;
        }

        [HttpPost]
        public ApiMessage GetFullInfo()
        {
            ApiMessage message = new ApiMessage() { Status="ok" };
            BProduct product=null;
            message.Item = product;
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            ProductManager pdtManager = new ProductManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            int product_id = 0;
            int.TryParse(request["product_id"],out product_id);
            try
            {
                product = pdtManager.GetProductFullInfo(product_id);
                message.Item = product;
            }
            catch (KM.JXC.Common.KMException.KMJXCException kex)
            {
            }

            return message;
        }

        [HttpPost]
        public PQGridData GetBuyOrders()
        {
            PQGridData data = new PQGridData();
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            BuyManager buyManager = new BuyManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            int total = 0;
            int page = 1;
            int pageSize = 30;
            int.TryParse(request["page"], out page);
            int.TryParse(request["pageSize"],out pageSize);
            data.data = buyManager.SearchBuyOrders(null,null, null, null, 0, 0, page, pageSize, out total);
            data.totalRecords = total;
            return data;
        }

        [HttpPost]
        public PQGridData GetBuys()
        {
            PQGridData data = new PQGridData();            
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            BuyManager buyManager = new BuyManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            int total = 0;
            int page = 1;
            int pageSize = 30;
            int.TryParse(request["page"], out page);
            int.TryParse(request["pageSize"], out pageSize);
            data.data = buyManager.SearchBuys(null,null,null,null,null,0,0,page,pageSize,out total);
            data.totalRecords = total;
            return data;
        }

        [HttpPost]
        public List<BBuyDetail> GetBuyDetails()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            BuyManager buyManager = new BuyManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            int buy_id = 0;
            int.TryParse(request["buy_id"],out buy_id);
            List<BBuyDetail> details = buyManager.GetBuyDetails(buy_id);
            return details;
        }

        [HttpPost]
        public ApiMessage EnterStockFromBuy()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            StockManager stockManager = new StockManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            BuyManager buyManager = new BuyManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            ApiMessage message = new ApiMessage();

            int buy_id = 0;
            string stockInfo = request["stocks"];
            int updateStock = 0;
            int shouseId = 0;

            int.TryParse(request["buy_id"],out buy_id);
            int.TryParse(request["update_stock"],out updateStock);
            int.TryParse(request["house_id"],out shouseId);
            try
            {
                BEnterStock stock = new BEnterStock();
                stock.BuyID = buy_id;
                stock.EnterTime = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                stock.StoreHouse = new Store_House() { Shop_ID = stockManager.Shop.Shop_ID, StoreHouse_ID = shouseId };
                
                if (!string.IsNullOrEmpty(stockInfo))
                {
                    stock.Details = new List<BEnterStockDetail>();
                    string[] ss = stockInfo.Split(';');
                    foreach (string s in ss)
                    {
                        string[] sss = s.Split(',');
                        foreach (string ssss in sss)
                        {
                            BEnterStockDetail detail = new BEnterStockDetail();
                            detail.Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                            detail.Quantity = int.Parse(ssss.Split(':')[1]);
                            detail.Price = double.Parse(ssss.Split(':')[2]);
                            detail.Product = new BProduct() {  ID=int.Parse(ssss.Split(':')[0])};
                            stock.Details.Add(detail);
                        }
                    }
                }

                stockManager.EnterStock(stock);
            }
            catch (JXC.Common.KMException.KMJXCException kex)
            {
                message.Status = "failed";
                message.Message = kex.Message;
            }
            finally
            {

            }

            return message;
        }

        [HttpPost]
        public ApiMessage CreateBuyOrder()
        {
            ApiMessage message = new ApiMessage();
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            BuyManager buyManager = new BuyManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            try
            {
                long writedate=0;
                long issuedate = 0;
                long enddate = 0;
                int supplier_id = 0;
                int order_user = 0;
                string odetails = request["order_products"];
                string desc=request["description"];
                
                if (!string.IsNullOrEmpty(request["writedate"]))
                {
                    writedate = DateTimeUtil.ConvertDateTimeToInt(Convert.ToDateTime(request["writedate"]));
                }
                
                if (!string.IsNullOrEmpty(request["issuedate"]))
                {
                    issuedate = DateTimeUtil.ConvertDateTimeToInt(Convert.ToDateTime(request["issuedate"]));
                }
                
                if (!string.IsNullOrEmpty(request["enddate"]))
                {
                    enddate = DateTimeUtil.ConvertDateTimeToInt(Convert.ToDateTime(request["enddate"]));
                }

                int.TryParse(request["supplier"],out supplier_id);
                int.TryParse(request["order_user"],out order_user);
                BBuyOrder order = new BBuyOrder();
                order.Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                order.Created_By = new BUser() { ID = buyManager.CurrentUser.ID };
                order.Description = desc;
                order.EndTime = enddate;
                order.InsureTime = issuedate;
                order.OrderUser = new BUser() { ID=order_user};
                order.Shop = new BShop() { ID=buyManager.Shop.Shop_ID};
                order.Status = 0;
                order.Supplier = new Supplier() {  Supplier_ID=supplier_id};
                order.WriteTime = writedate;
                if (!string.IsNullOrEmpty(odetails))
                {
                    order.Details = new List<BBuyOrderDetail>();
                    string[] details = odetails.Split(';');
                    foreach (string detail in details)
                    {
                        string[] items = detail.Split(',');
                        BBuyOrderDetail oDetail = new BBuyOrderDetail();
                        oDetail.Price = decimal.Parse(items[2]);
                        oDetail.Product = new BProduct() { ID = int.Parse(items[0]) };
                        oDetail.Quantity = int.Parse(items[1]);
                        oDetail.Status = 0;
                        order.Details.Add(oDetail);
                    }
                }

                bool result = buyManager.CreateNewBuyOrder(order);
                if (result)
                {
                    message.Status = "ok";
                }
                else {
                    message.Status = "failed";
                    message.Message = "创建失败";
                }
            }
            catch (KM.JXC.Common.KMException.KMJXCException kex)
            {
                message.Status = "failed";
                message.Message = kex.Message;
            }
            catch (Exception ex)
            {
                message.Status = "failed";
                message.Message = ex.Message;
            }
            finally
            {

            }

            return message;
        }

        [HttpPost]
        public ApiMessage UpdateBuyOrder()
        {
            ApiMessage message = new ApiMessage();
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            BuyManager buyManager = new BuyManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            try
            {
                int oid = 0;
                long writedate = 0;
                long issuedate = 0;
                long enddate = 0;
                int supplier_id = 0;
                int order_user = 0;
                string odetails = request["order_products"];
                string desc = request["description"];
                int.TryParse(request["oid"],out oid);
                if (!string.IsNullOrEmpty(request["writedate"]))
                {
                    writedate = DateTimeUtil.ConvertDateTimeToInt(Convert.ToDateTime(request["writedate"]));
                }

                if (!string.IsNullOrEmpty(request["issuedate"]))
                {
                    issuedate = DateTimeUtil.ConvertDateTimeToInt(Convert.ToDateTime(request["issuedate"]));
                }

                if (!string.IsNullOrEmpty(request["enddate"]))
                {
                    enddate = DateTimeUtil.ConvertDateTimeToInt(Convert.ToDateTime(request["enddate"]));
                }

                int.TryParse(request["supplier"], out supplier_id);
                int.TryParse(request["order_user"], out order_user);
                BBuyOrder order = new BBuyOrder();
                order.ID = oid;
                //order.Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                //order.Created_By = new BUser() { ID = buyManager.CurrentUser.ID };
                order.Description = desc;
                order.EndTime = enddate;
                order.InsureTime = issuedate;
                order.OrderUser = new BUser() { ID = order_user };
                order.Shop = new BShop() { ID = buyManager.Shop.Shop_ID };
                order.Status = 0;
                order.Supplier = new Supplier() { Supplier_ID = supplier_id };
                order.WriteTime = writedate;
                if (!string.IsNullOrEmpty(odetails))
                {
                    order.Details = new List<BBuyOrderDetail>();
                    string[] details = odetails.Split(';');
                    foreach (string detail in details)
                    {
                        string[] items = detail.Split(',');
                        BBuyOrderDetail oDetail = new BBuyOrderDetail();
                        oDetail.Price = decimal.Parse(items[2]);
                        oDetail.Product = new BProduct() { ID = int.Parse(items[0]) };
                        oDetail.Quantity = int.Parse(items[1]);
                        oDetail.Status = 0;
                        order.Details.Add(oDetail);
                    }
                }

                bool result = buyManager.UpdateBuyOrder(order);
                if (result)
                {
                    message.Status = "ok";
                }
                else
                {
                    message.Status = "failed";
                    message.Message = "更新失败";
                }
            }
            catch (KM.JXC.Common.KMException.KMJXCException kex)
            {
                message.Status = "failed";
                message.Message = kex.Message;
            }
            catch (Exception ex)
            {
                message.Status = "failed";
                message.Message = ex.Message;
            }
            finally
            {

            }

            return message;
        }

        [HttpPost]
        public ApiMessage VerifyOrder()
        {
            ApiMessage message = new ApiMessage();
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string user_id = User.Identity.Name;
            UserManager userMgr = new UserManager(int.Parse(user_id), null);
            BUser user = userMgr.CurrentUser;
            BuyManager buyManager = new BuyManager(userMgr.CurrentUser, userMgr.Shop, userMgr.CurrentUserPermission);
            try
            {
                int oid = 0;
                long comeDate = 0;
                string odetails = request["order_products"];
                string desc = request["description"];
                int.TryParse(request["oid"], out oid);

                if (!string.IsNullOrEmpty(request["comedate"]))
                {
                    comeDate = DateTimeUtil.ConvertDateTimeToInt(Convert.ToDateTime(request["comedate"]));
                }

                BBuy buy = new BBuy();
                buy.ID = 0;
                buy.Order = new BBuyOrder() { ID = oid };
                buy.ComeDate = comeDate;
                buy.Description = desc;
                buy.Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                buy.Shop = new BShop() { ID = buyManager.Shop.Shop_ID };
                buy.User = new BUser() { ID = buyManager.CurrentUser.ID };
               
                if (!string.IsNullOrEmpty(odetails))
                {
                    buy.Details = new List<BBuyDetail>();
                    string[] details = odetails.Split(';');
                    foreach (string detail in details)
                    {
                        string[] items = detail.Split(',');
                        BBuyDetail oDetail = new BBuyDetail();
                        oDetail.Buy_Order_ID = oid;
                        oDetail.Price = decimal.Parse(items[2]);
                        oDetail.Product = new BProduct() { ID = int.Parse(items[0]) };
                        oDetail.Quantity = int.Parse(items[1]);                       
                        buy.Details.Add(oDetail);
                    }
                }

                bool result = buyManager.VerifyBuyOrder(buy);
                
                if (result)
                {
                    message.Status = "ok";
                }
                else
                {
                    message.Status = "failed";
                    message.Message = "验货单创建失败";
                }
            }
            catch (KM.JXC.Common.KMException.KMJXCException kex)
            {
                message.Status = "failed";
                message.Message = kex.Message;
            }
            catch (Exception ex)
            {
                message.Status = "failed";
                message.Message = ex.Message;
            }
            finally
            {

            }

            return message;
        }
    }
}