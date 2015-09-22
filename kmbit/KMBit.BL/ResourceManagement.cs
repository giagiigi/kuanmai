﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMBit.DAL;
using KMBit.Beans;
using KMBit.Util;

namespace KMBit.BL
{
    public class ResourceManagement:BaseManagement
    {
        public ResourceManagement(int userId):base(userId)
        {
            if (this.logger == null)
            {
                this.logger = log4net.LogManager.GetLogger(typeof(ResourceManagement));
            }
        }

        public List<BResource> FindResource(int resourceId,string resourceName)
        {
            List<BResource> resources = null;
            using (chargebitEntities db = new chargebitEntities())
            {
                var tmp = from s in db.Resource
                          join sp in db.Sp on s.SP_Id equals sp.Id into lsp
                          from llsp in lsp.DefaultIfEmpty()
                          join pa in db.Area on s.Province_Id equals pa.Id into lpa
                          from llpa in lpa.DefaultIfEmpty()
                          join ca in db.Area on s.City_Id equals ca.Id into lca
                          from llca in lca.DefaultIfEmpty()
                          select new BResource
                          {
                              Resource = new Resource()
                              {
                                  Address = s.Address,
                                  City_Id = s.City_Id,
                                  Contact = s.Contact,
                                  CreatedBy = s.CreatedBy,
                                  Created_time = s.Created_time,
                                  Description = s.Description,
                                  Email = s.Email,
                                  Enabled = s.Enabled,
                                  Id = s.Id,
                                  Name = s.Name,
                                  Province_Id = s.Province_Id,
                                  SP_Id = s.SP_Id,
                                  UpdatedBy = s.UpdatedBy,
                                  Updated_time = s.Updated_time
                              },
                              City = new Area() { Id = s.City_Id != null ? (int)s.City_Id : 0, Name = llca != null ? llca.Name : "" },
                              Province = new Area() { Id = s.Province_Id, Name = llpa != null ? llpa.Name : "" },
                              SP = llsp != null ? new Sp() { Name = llsp.Name, Id = s.SP_Id } : null
                          };
                if(resourceId>0)
                {
                    tmp = tmp.Where(s=>s.Resource.Id==resourceId);
                }
                if(!string.IsNullOrEmpty(resourceName))
                {
                    tmp = tmp.Where(s=>s.Resource.Name.Contains(resourceName));
                }

                tmp.OrderBy(s => s.Resource.Created_time);

                resources = tmp.ToList<BResource>();
            }
            return resources;
        }

        public bool CreateResource(Resource resource)
        {
            bool ret = false;

            if(CurrentLoginUser.Permission.CREATE_RESOURCE==0)
            {
                throw new KMBitException("没有权限创建资源");
            }

            if (resource == null)
            {
                logger.Error("resource is NULL");
                throw new KMBitException("资源输入参数不正确");
            }

            if (string.IsNullOrEmpty(resource.Name))
            {
                logger.Error("resource name cannot be empty");
                throw new KMBitException("资源名称不能为空");
            }
            resource.Enabled = true;
            resource.Created_time = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
            resource.Updated_time = resource.Created_time;
            using (chargebitEntities db = new chargebitEntities())
            {
                db.Resource.Add(resource);
                db.SaveChanges();
                ret = true;
            }

            return ret;
        }

        public bool UpdateResource(Resource resource)
        {
            if (CurrentLoginUser.Permission.UPDATE_RESOURCE == 0)
            {
                throw new KMBitException("没有权限更新资源");
            }
            bool ret = false;
            if (resource == null)
            {
                logger.Error("resource is NULL");
                throw new KMBitException("资源输入参数不正确");
            }
            if (resource.Id <= 0)
            {
                throw new KMBitException("资源输入参数不正确");
            }
            if (string.IsNullOrEmpty(resource.Name))
            {
                logger.Error("resource name cannot be empty");
                throw new KMBitException("资源名称不能为空");
            }
            using (chargebitEntities db = new chargebitEntities())
            {
                resource.Created_time = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                db.Resource.Attach(resource);               
                db.SaveChanges();
                ret = true;
            }

            return ret;
        }

        public bool CreateResourceTaocan(Resource_taocan taocan)
        {
            bool ret = false;
            if (CurrentLoginUser.Permission.CREATE_RESOURCE_TAOCAN == 0)
            {
                throw new KMBitException("没有权限创建资源套餐");
            }

            if (taocan.Quantity <= 0)
            {
                throw new KMBitException("套餐容量不能为零");
            }

            taocan.Enabled = true;
            taocan.Created_time = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
            taocan.Updated_time = taocan.Created_time;
            using (chargebitEntities db = new chargebitEntities())
            {
                Taocan ntaocan = (from t in db.Taocan where t.Sp_id == taocan.Sp_id && t.Quantity == taocan.Quantity select t).FirstOrDefault<Taocan>();
                Sp sp = (from s in db.Sp where s.Id == taocan.Sp_id select s).FirstOrDefault<Sp>();
                if (ntaocan == null)
                {
                    string taocanName = sp != null ? sp.Name + " " + taocan.Quantity.ToString() + "M" : "全网 " + taocan.Quantity.ToString() + "M";
                    ntaocan = new Taocan() { Created_time = taocan.Created_time, Description = taocanName, Name = taocanName, Sp_id = taocan.Sp_id, Quantity = taocan.Quantity, Updated_time = taocan.Updated_time };
                    db.Taocan.Add(ntaocan);
                    db.SaveChanges();
                }
                if (ntaocan.Id > 0)
                {
                    db.Resource_taocan.Add(taocan);
                    db.SaveChanges();
                    ret = true;
                }
                else
                {
                    throw new KMBitException("套餐创建失败");
                }

            }
            return ret;
        }

        public List<Resource_taocan> FindResourceTaocans(int sTaocanId,int resourceId,int spId)
        {
            List<Resource_taocan> sTaocans=null;
            return sTaocans;
        }
    }
}
