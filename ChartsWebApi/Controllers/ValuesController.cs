using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Newtonsoft.Json.Linq;
using ChartsWebApi.ViewModel;
using Microsoft.AspNetCore.Cors;

namespace ChartsWebApi.Controllers
{
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5/index
        [HttpGet("{id}")]
        public JsonResult Get(int id, string index)
        {
            ChartDbContext dbcontext = new ChartDbContext();
            List<FunctionRel> list = null;
            if (id == 0)
            {
                return Json(list);
            }
            if (string.IsNullOrEmpty(index))
            {
                index = "默认";
            }
            List<FunctionRel> rowIndexs = dbcontext.FunctionRels.Where(fr => fr.Funcid == id).ToList();
            List<FunctionValue> columnValues = dbcontext.FunctionValues.Where(fv => fv.FuncId == id).ToList();
            List<string> columns = new List<string>();
            List<JObject> rows = new List<JObject>();
            ChartDataModel model = new ChartDataModel
            {
                columns = columns,
                rows = rows
            };
            columns.Add(index);
            columns.AddRange(from cv in columnValues select cv.Level2Name);
            //columns = ['Index','张三','李四','王五']
            //columnValues = [
            //  { Level2Name: '张三', V0001: 100, V0002: 87 },
            //  { Level2Name: '李四', V0001: 85, V0002: 76 },
            //  { Level2Name: '王五', V0001: 91, V0002: 99 }
            //]
            for (int i = 0, j = rowIndexs.Count; i < j; i++)
            {
                //第一行 row = { funcAttrName: '一月', valueAttrName: 'CN_V0001' }
                //第二行 row = { funcAttrName: '二月', valueAttrName: 'CN_V0002' }
                var row = rowIndexs[i];
                var obj = new JObject();
                obj.Add(columns[0], row.FuncattrName);
                for (int x = 0, y = columnValues.Count; x < y; x++)
                {
                    var column = columnValues[x];
                    //第一轮 obj = {Index: '一月', '张三': 'CN_V0001'}
                    //第二轮 obj = {Index: '一月', '张三': 'CN_V0001', '李四': 'CN_V0001'}
                    //第三轮 obj = {Index: '一月', '张三': 'CN_V0001', '李四': 'CN_V0001', '王五': 'CN_V0001'}
                    obj.Add(column.Level2Name, column.GetType().GetProperty(row.ValueattrName.Replace("CN_","")).GetValue(column).ToString());
                }
                rows.Add(obj);
            }
            return Json(model);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
