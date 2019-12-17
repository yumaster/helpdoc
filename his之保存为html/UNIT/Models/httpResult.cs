namespace UNIT
{
    /// <summary>
    /// Http响应结果
    /// </summary>
    public class httpResult
    {
        public int status { get; set; }//状态码
        public object result { set; get; }//响应结果
                                          /// <summary>
                                          /// 实例化
                                          /// </summary>
        public static httpResult instance { get { return new httpResult(); } }
        /// <summary>
        /// 构造函数
        /// </summary>
        public httpResult()
        {
            status = 0;
            result = "成功";
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="status"></param>
        /// <param name="result"></param>
        public httpResult(int status, object result)
        {
            this.status = status;
            this.result = result;
        }
    }   
}