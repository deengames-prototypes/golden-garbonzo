namespace Prototype.Game.Models.Items
{
    abstract class AbstractItem
    {
        public string Name {  get {
                return this.GetType().Name;
        } }

        public abstract string Description { get; }
    }
}
