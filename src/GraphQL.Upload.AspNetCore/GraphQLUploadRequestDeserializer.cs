using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace GraphQL.Upload.AspNetCore
{
    public class GraphQLUploadRequestDeserializer
    {
        public GraphQLUploadRequestDeserializationResult DeserializeFromFormCollection(IFormCollection form)
        {
            var result = new GraphQLUploadRequestDeserializationResult { IsSuccessful = true };

            SetOperations(result, form);
            SetMap(result, form);

            return result;
        }

        private void SetOperations(GraphQLUploadRequestDeserializationResult result, IFormCollection form)
        {
            if (!form.TryGetValue("operations", out var operations))
            {
                throw new Exception("Missing field 'operations'.");
            }

            var firstChar = operations[0][0];
            bool isBatched = firstChar == '[';

            try
            {
                if (isBatched)
                {
                    result.Batch = ((System.Text.Json.JsonSerializer.Deserialize<GraphQLUploadRequest[]>(operations)) ?? Array.Empty<GraphQLUploadRequest>())
                        .ToArray();
                }
                else
                {
                    result.Single = System.Text.Json.JsonSerializer.Deserialize<GraphQLUploadRequest>(operations);
                }
            }
            catch
            {
                throw new Exception("Invalid JSON in the 'operations' Upload field.");
            }
        }

        private void SetMap(GraphQLUploadRequestDeserializationResult result, IFormCollection form)
        {
            if (!form.TryGetValue("map", out var map))
            {
                throw new Exception("Missing field 'map'");
            }

            try
            {
                result.Map = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(map);
            }
            catch (JsonException)
            {
                throw new Exception("Invalid JSON in the 'map' Upload field.");
            }
        }
    }
}
