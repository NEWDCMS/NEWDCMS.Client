namespace Wesley.Client.Validations
{

    /// <summary>
    /// 自定义验证规则
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValidationRule<T>
    {
        /// <summary>
        /// 验证消息
        /// </summary>
        string ValidationMessage { get; set; }

        /// <summary>
        /// 检查输入
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Check(T value);
    }
}
