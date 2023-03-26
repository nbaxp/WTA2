using Microsoft.AspNetCore.Mvc;

namespace WTA.Application.Application;

public class BaseTreeModel<T> where T : class
{
    public Guid Id { get; set; }
    public T? Parent { get; set; }

    [HiddenInput]
    public Guid? ParentId { get; set; }

    public List<T> Children { get; set; } = new List<T>();
}

public static class BaseTreeModelExtensions
{
    public static List<T> IdentityResolution<T>(this List<T> list) where T : BaseTreeModel<T>
    {
        foreach (var item in list)
        {
            if (item.ParentId.HasValue)
            {
                item.Parent = list.FirstOrDefault(o => o.Id == item.ParentId);
                item.Parent?.Children.Add(item);
            }
        }
        return list.Where(o => o.ParentId == null).ToList();
    }
}
