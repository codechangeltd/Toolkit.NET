namespace CodeChange.Toolkit
{
    /// <summary>
    /// <a href="https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern">CRTP</a>
    /// based interface to implement for objects that can create deep clones of themselves,
    /// but can be abused if TSelf is not specified as the same type as the implementing class.
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <remarks>
    /// This idea was copied from https://paulsebastian.codes/a-solution-to-deep-cloning-in-csharp
    /// </remarks>
    public interface IDeepCloneable<TSelf> where TSelf : IDeepCloneable<TSelf>
    {
        public TSelf DeepClone();
    }
}
