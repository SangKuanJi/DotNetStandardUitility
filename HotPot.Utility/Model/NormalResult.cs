namespace HotPot.Utility.Model
{
    public class NormalResult
    {

        public NormalResult()
        {
        }

        public NormalResult(string message)
        {
            this.Message = message;
            this.Successful = true;
        }

        public NormalResult(bool successful, string message)
        {
            this.Message = message;
            this.Successful = successful;
        }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// 用于根据不同的原因进行不同的处理
        /// </summary>
        public int Reason { get; set; }
    }

    public class NormalResult<T> : NormalResult
    {
        public NormalResult() { }
        public NormalResult(bool successful, string message) : base(successful, message) { }

        public NormalResult(T data)
        {
            this.Successful = true;
            this.Data = data;
        }
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }
    }
}
