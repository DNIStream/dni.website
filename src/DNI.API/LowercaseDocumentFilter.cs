using System.Collections.Generic;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DNI.API {
    /// <summary>
    ///     Generates swagger documentation using lowercase URIs
    /// </summary>
    public class LowercaseDocumentFilter : IDocumentFilter {
        /// <summary>
        ///     Generates swagger documentation using lowercase URIs
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context) {
            var paths = swaggerDoc.Paths;

            //	generate the new keys
            var newPaths = new Dictionary<string, PathItem>();
            var removeKeys = new List<string>();
            foreach(var path in paths) {
                var newKey = path.Key.ToLower();
                if(newKey != path.Key) {
                    removeKeys.Add(path.Key);
                    newPaths.Add(newKey, path.Value);
                }
            }

            //	add the new keys
            foreach(var path in newPaths) {
                swaggerDoc.Paths.Add(path.Key, path.Value);
            }

            //	remove the old keys
            foreach(var key in removeKeys) {
                swaggerDoc.Paths.Remove(key);
            }
        }
    }
}