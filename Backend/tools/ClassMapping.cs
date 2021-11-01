using Newtonsoft.Json;

namespace Backend.tools
{
    public class ClassMapping
    {
        /**
         * <summary>helper method to map domain class to dto class or vice versa
         * <example>For example:
         * <code>
         *  UserProfile profile = new UserProfile();
         *  UserProfileResponse response = ConvertDomainDto&lt;UserProfile, UserProfileResponse&gt;(profile);
         * </code>
         * </example>
         * </summary>
         *
         * <typeparam name="TSourceCls">The source class</typeparam>
         * <typeparam name="TTargetCls">the target class</typeparam>
         *
         * <param name="inputObject">the input object</param>
         *
         * <returns>the <paramref name="inputObject"/> as a <typeparamref name="TTargetCls"/> class </returns>
         */
        public static TTargetCls ConvertDomainDto<TSourceCls, TTargetCls>(TSourceCls inputObject)
        {
            // ok this is a bit dirty but it works
            var jsonOfSourceCls = JsonConvert.SerializeObject(inputObject);

            return JsonConvert.DeserializeObject<TTargetCls>(jsonOfSourceCls);
        }
    }
}