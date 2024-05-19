
using Microsoft.AspNetCore.Http;

namespace Appointment.SDK.Backend.Utilities;

public static class QueryCollectionExtension
{
    public static object[] GetPropertiesByParams(this IQueryCollection queryCollection, Type ObjType)
    {
        var Properties = ObjType.GetProperties()
            .Where(x => queryCollection.ContainsKey(x.Name))
            .ToList();

        return null!;
    }
}