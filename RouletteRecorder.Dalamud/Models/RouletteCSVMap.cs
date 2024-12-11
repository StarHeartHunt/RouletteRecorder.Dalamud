using CsvHelper.Configuration;
using RouletteRecorder.Dalamud.DAO;

namespace RouletteRecorder.Dalamud.Models
{
    internal sealed class RouletteCsvMap : ClassMap<Roulette>
    {
        public RouletteCsvMap()
        {
            Map(m => m.RouletteType).Name("任务类型");
            Map(m => m.Date).Name("日期");
            Map(m => m.StartedAt).Name("开始时间");
            Map(m => m.EndedAt).Name("结束时间");
            Map(m => m.ContentName).Name("副本名称");
            Map(m => m.JobName).Name("职业");
            Map(m => m.IsCompleted).Name("完成情况");
        }
    }
}
