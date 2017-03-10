using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbShow.Dal;
using MetaDataV2;
using MvcDBShow.Models;

using System.Text.RegularExpressions;

namespace MvcDBShow.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            HomeModel model = new HomeModel();
            var list = new List<TabEnt>();

            new T4Base(AppDomain.CurrentDomain.BaseDirectory, "DBName");
            foreach (TableObject current in T4Base.MyDb.GetTableView())
            {
                Console.WriteLine(current.name);
                var entTmp = new TabEnt() { TabName = current.name, TabRemarks = current.comments };

                var listCol = new List<ColEnt>();

                for (int index = 0; index < current.Columns.Count; index++)
                {
                    var col = current.Columns[index];
                    ColEnt colTmp = new ColEnt()
                    {
                        ColName = col.ColumnName,
                        ColRemarks = col.deText,
                        ColType = col.DbTypeNameStr
                    };
                    listCol.Add(colTmp);
                }

                entTmp.TabCol = listCol;

                list.Add(entTmp);
            }
            model.TableList = list;
            return View(model);
        }

    }
}
