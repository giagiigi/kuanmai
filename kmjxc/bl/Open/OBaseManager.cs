﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KM.JXC.DBA;
using Top.Api;

namespace KM.JXC.BL.Open
{
    public class OBaseManager
    {
        public Mall_Type MallType { get; set; }
        public Access_Token Access_Token { get; set; }
        public Open_Key Open_Key { get; set; }
        public ITopClient client {get;set;}
        public int Mall_Type_ID { get; private set; }

        public OBaseManager(int mall_type_id,Access_Token token)
        {
            this.Mall_Type_ID = mall_type_id;
            this.Access_Token = token;
            this.Open_Key = this.GetAppKey();
            this.MallType = this.GetMallType();
            client = new DefaultTopClient(this.Open_Key.API_Main_Url, this.Open_Key.AppKey, this.Open_Key.AppSecret, "json");
        }
        public OBaseManager(int mall_type_id)
        {
            this.Mall_Type_ID = mall_type_id;          
            this.Open_Key = this.GetAppKey();
            this.MallType = this.GetMallType();
            client = new DefaultTopClient(this.Open_Key.API_Main_Url, this.Open_Key.AppKey, this.Open_Key.AppSecret, "json");
        }

        protected Open_Key GetAppKey()
        {
            Open_Key key = null;
            if (this.Mall_Type_ID <= 0)
            {
                throw new Exception("Mall Type ID is invalid for open API");
            }

            try
            {
                KuanMaiEntities db = new KuanMaiEntities();
                var openKey = db.Open_Key.Where(p => p.Mall_Type_ID == this.Mall_Type_ID);
                if (openKey != null)
                {
                    List<Open_Key> keys = openKey.ToList<Open_Key>();
                    if (keys.Count == 1)
                    {
                        key = keys[0];
                    }
                    else
                    {
                        throw new Exception("More app key and secret pair for Mall Type ID:" + this.Mall_Type_ID);
                    }
                }
                else
                {
                    throw new Exception("Didn't find app key and secret for Mall Type ID:" + this.Mall_Type_ID);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Nothing to do
            }

            return key;
        }

        protected Mall_Type GetMallType()
        {
            Mall_Type type = null;
            using (KuanMaiEntities db = new KuanMaiEntities())
            {
                var t = from tp in db.Mall_Type where tp.Mall_Type_ID == this.Mall_Type_ID select tp;
                if (t.ToList<Mall_Type>().Count == 1)
                {
                    type = t.ToList<Mall_Type>()[0];
                }
            }

            return type;
        }        
    }
}