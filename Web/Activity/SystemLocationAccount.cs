using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UMC.Data;

namespace UMC.Web.Activity
{
    class SystemLocationActivity : Web.Activity.CSVActivity
    {
        String _title;
        public override string Title
        {
            get { return "上传地区"; }
        }
        List<String> _Columns = new List<string>();

        public override string[] Columns
        {
            get
            {
                return new string[] { "Id", "ParentId", "Type", "ZipCode" };
            }
        }

        protected string Type { get; set; }

        public String Username;

        public override void ProcessActivity(WebRequest request, WebResponse response)
        {

            base.ProcessActivity(request, response);
        }



        protected override bool Do(CSV.Log log, int rowIndex, Hashtable value)
        {
            if (this.Type == "Store")
            {
                var product_Code = value["系列编码"] as string;
                var store_Code = value["门店编码"] as string;
                var staff_Code = value["工号"] as string;
                if (String.IsNullOrEmpty(product_Code) || String.IsNullOrEmpty(store_Code) || String.IsNullOrEmpty(staff_Code))
                {
                    log.Error(String.Format("第{0}行 数据无效", rowIndex));


                    return false; ;
                }
                var store = Utility.Database.ObjectEntity<BO_EU_STORE>()
                                 .Where.And().Equal(new BO_EU_STORE { STORE_CODE = store_Code }).Entities.Single();
                if (store == null)
                {
                    log.Error(String.Format("第{0}行 未找到“{1}”门店", rowIndex, store_Code));
                    return false; ;
                }


                var line = Utility.Database.ObjectEntity<BO_EU_STORE_PRODUCT_LINE>()
                                 .Where.And().Equal(new BO_EU_STORE_PRODUCT_LINE
                                 {
                                     BINDID = store.BINDID,
                                     PRODUCT_LINE_CODE = product_Code,
                                     IS_ACTIVE = 1
                                 }).Entities.Single();
                if (line == null)
                {
                    log.Error(String.Format("第{0}行 未找到“{2}”门店的有效的“{1}”产品编码", rowIndex, product_Code, store_Code));
                    return false; ;
                }

                var orgUser = Utility.Database.ObjectEntity<ORGUSER>()
                          .Where.And().Equal(new ORGUSER { USERID = staff_Code }).Entities.Single();

                if (orgUser == null)
                {
                    log.Error(String.Format("第{0}行 未找到“{1}”工号", rowIndex, staff_Code));
                    return false; ;
                }


                var superEntity = Utility.Database.ObjectEntity<Entities.UMC_STORE_SUPERVISOR>()
                        .Where.And().In(new UMC_STORE_SUPERVISOR { PRODUCT_LINE_ID = line.ID })
                        .Entities;
                var super = superEntity.Single();
                if (super != null)
                {
                    if (super.STAFF_CODE == orgUser.USERID)
                    {
                        return true;
                    }
                    superEntity.Update(new UMC_STORE_SUPERVISOR { STAFF_CODE = orgUser.USERID, CHANGE_TIME = DateTime.Now });


                    DDing.Send(super.STAFF_CODE, String.Format("管理员帮您移交({1}){0}", store.STORE_NAME, store.STORE_CODE, line.PRODUCT_SERIES, orgUser.USERNAME, orgUser.USERID));

                    DDing.Send(orgUser.USERID, String.Format("管理员帮您认领({1}){0}", store.STORE_NAME, store.STORE_CODE, line.PRODUCT_SERIES, orgUser.USERNAME, orgUser.USERID));
                    var app = new UMC_APPLY_SUPERVISOR()
                    {
                        APPLY_STAFF_CODE = Username,
                        CHANGE_STAFF_CODE = orgUser.USERID,
                        GROUP_ID = Guid.NewGuid(),
                        STAFF_CODE = Username,//super.STAFF_CODE,
                        STORE_NAME = store.STORE_NAME,
                        STORE_CODE = store.STORE_CODE,
                        ORIGINAL_STAFF_CODE = super.STAFF_CODE,
                        PRODUCT_LINE_ID = line.ID,
                        PRODUCT_LINE_CODE = line.PRODUCT_LINE_CODE,
                        PRODUCT_LINE_NAME = line.PRODUCT_SERIES,
                        TITLE = "管理员操作",
                        CHANGE_TIME = DateTime.Now,
                        Type = 0,
                        CREATION_TIME = DateTime.Now,
                        STATUS = 1

                    };

                    Utility.Database.ObjectEntity<UMC_APPLY_SUPERVISOR>()
                           .Insert(app);
                }
                else
                {
                    superEntity.Insert(new UMC_STORE_SUPERVISOR
                    {
                        Id = line.ID,
                        PRODUCT_LINE_ID = line.ID,
                        CHANGE_TIME = DateTime.Now,
                        PRODUCT_LINE_CODE = line.PRODUCT_LINE_CODE,
                        PRODUCT_LINE_NAME = line.PRODUCT_SERIES,
                        STAFF_CODE = orgUser.USERID,
                        STATUS = 16,
                        STORE_CODE = store.STORE_CODE
                    });
                    DDing.Send(orgUser.USERID, String.Format("管理员帮您认领({1}){0}", store.STORE_NAME, store.STORE_CODE, line.PRODUCT_SERIES, orgUser.USERNAME, orgUser.USERID));
                    var app = new UMC_APPLY_SUPERVISOR()
                    {
                        APPLY_STAFF_CODE = Username,
                        CHANGE_STAFF_CODE = orgUser.USERID,
                        GROUP_ID = Guid.NewGuid(),
                        STAFF_CODE = Username,//super.STAFF_CODE,
                        STORE_NAME = store.STORE_NAME,
                        STORE_CODE = store.STORE_CODE,
                        PRODUCT_LINE_ID = line.ID,
                        PRODUCT_LINE_CODE = line.PRODUCT_LINE_CODE,
                        PRODUCT_LINE_NAME = line.PRODUCT_SERIES,
                        CHANGE_TIME = DateTime.Now,
                        TITLE = "管理员操作",
                        Type = 0,
                        CREATION_TIME = DateTime.Now,
                        STATUS = 1

                    };

                    Utility.Database.ObjectEntity<UMC_APPLY_SUPERVISOR>()
                           .Insert(app);

                }
            }
            else
            {
                var depName = value["部门"] as string;
                var staff_Code = value["工号"] as string;



                var orgUser = Utility.Database.ObjectEntity<ORGUSER>()
                          .Where.And().Equal(new ORGUSER { USERID = staff_Code }).Entities.Single();

                if (orgUser == null)
                {
                    log.Error(String.Format("第{0}行 未找到工号", rowIndex));
                    return false; ;
                }
                var depNames = new List<string>(depName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
                if (depNames.Count == 0)
                {
                    log.Error(String.Format("第{0}行 部门不正确", rowIndex));
                    return false;
                }
                ORGDEPARTMENT dep = null;
                var depEntity = Utility.Database.ObjectEntity<ORGDEPARTMENT>();
                var parentId = new List<String>(); ;
                while (depNames.Count > 0)
                {
                    depEntity.Where.Reset().And().Equal(new ORGDEPARTMENT
                    {
                        DEPARTMENTNAME = depNames[0]
                    });
                    if (parentId.Count > 0)
                    {
                        depEntity.Where.And().In(new ORGDEPARTMENT { PARENTDEPARTMENTID = parentId[0] }, parentId.ToArray());

                    }
                    parentId.Clear();
                    depEntity.Query(dr =>
                    {
                        dep = dr;
                        parentId.Add(dr.ID.ToString());
                    });
                    if (parentId.Count == 0)
                    {
                        log.Error(String.Format("第{0}行 {1}部门不正确", rowIndex, depNames[0]));
                        return false;
                    }
                    depNames.RemoveAt(0);

                }
                if (dep == null)
                {
                    log.Error(String.Format("第{0}行 未找到部门", rowIndex));
                    return false; ;
                }
                depEntity.Where.Reset().And().Equal(new ORGDEPARTMENT { PARENTDEPARTMENTID = dep.ToString(), CLOSED = 0 });//.Entities;//.Single();

                var dePids = new List<String>();
                depEntity.Query(dr => dePids.Add(dr.ID.ToString()));
                if (dePids.Count > 0)
                {
                    var count2 = depEntity.Where.Reset().And().Equal(new ORGDEPARTMENT { CLOSED = 0 }).And().In(new ORGDEPARTMENT
                    {
                        PARENTDEPARTMENTID = dePids[0]
                    }, dePids.ToArray())
                           .Entities.Count();
                    if (count2 > 0)
                    {
                        log.Error(String.Format("第{0}行 部门未细化", rowIndex));
                        return false; ;
                    }

                }
                var areaEntity = Utility.Database.ObjectEntity<UMC_AREA_SUPERVISOR>();
                var super = areaEntity.Where.Reset().And().Equal(new UMC_AREA_SUPERVISOR
                {
                    DEPARTMENTID = dep.ID,
                    ISADMIN = 1
                }).Entities.Single();
                if (super != null)
                {
                    if (super.STAFF_CODE == orgUser.USERID)
                    {
                        return true;
                    }
                    areaEntity.Delete();
                }


                var area_super = new UMC_AREA_SUPERVISOR
                {
                    Id = Guid.NewGuid(),
                    ISADMIN = 1,
                    AREA_NAME = dep.DEPARTMENTNAME,
                    DEPARTMENTID = dep.ID,
                    CHANGE_TIME = DateTime.Now,
                    STAFF_CODE = orgUser.USERID,
                    STATUS = 16,
                    SYNC_TIME = DateTime.Now
                };
                areaEntity.Insert(area_super);

                //switch (orgUser.USERID)
                //{
                //    case "6006345":
                //    case "6005942":
                //    case "6006211":
                //    case "6006030":
                //    case "6004383":
                //    case "6003578":
                //    case "6004363":
                //        break;
                //    default:
                DDing.Send(orgUser.USERID, String.Format("管理员帮您认领{0}", dep.DEPARTMENTNAME));
                //        break;
                //}

                var app = new UMC_APPLY_SUPERVISOR()
                {
                    APPLY_STAFF_CODE = Username,
                    GROUP_ID = Guid.NewGuid(),
                    CHANGE_STAFF_CODE = orgUser.USERID,
                    STAFF_CODE = Username,
                    STORE_NAME = dep.DEPARTMENTNAME,
                    STORE_CODE = dep.ID.ToString(),
                    PRODUCT_LINE_ID = area_super.Id,
                    Type = 1,
                    CHANGE_TIME = DateTime.Now,
                    CREATION_TIME = DateTime.Now,
                    STATUS = 1

                };
                if (super != null)
                {
                    app.ORIGINAL_STAFF_CODE = super.STAFF_CODE;

                    DDing.Send(super.STAFF_CODE, String.Format("管理员帮您移交{0}", super.AREA_NAME));
                }

                Utility.Database.ObjectEntity<UMC_APPLY_SUPERVISOR>()
                       .Insert(app);
            }
            return true;

        }
    }
}
