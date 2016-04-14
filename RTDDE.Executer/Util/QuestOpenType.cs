namespace RTDDE.Executer.Util
{
    public class QuestOpenType
    {
        public string Type { get; set; }
        public string Param { get; set; }
        public int Group { get; set; }
        public QuestOpenType()
        {
            Type = "未知";
            Param = string.Empty;
        }
        public QuestOpenType(string type, string param, int group)
        {
            Type = type;
            Param = param;
            Group = group;
        }
    }
}