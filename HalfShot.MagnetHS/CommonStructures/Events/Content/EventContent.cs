namespace HalfShot.MagnetHS.CommonStructures.Events.Content
{
    public interface IEventContent
    {
        void FromJsonContent(string json);
        /// <summary>
        /// Returns the 'true' representation of an event for putting into a json object.
        /// </summary>
        /// <returns></returns>
        object ToCanonicalObject();
    }
}