using Microsoft.Extensions.Caching.Distributed;
using WTA.Application.Abstractions;
using WTA.Application.Extensions;
using WTA.Application.Identity.Domain;

namespace WTA.Application.Identity.Services.Menus;

[Service<IMenuService>]
public class MenuService : IMenuService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<MenuItem> _menuItemRepository;
    private readonly IDistributedCache _distributedCache;

    public MenuService(IRepository<User> userRepository, IRepository<MenuItem> menuItemRepository, IDistributedCache distributedCache)
    {
        this._userRepository = userRepository;
        this._menuItemRepository = menuItemRepository;
        this._distributedCache = distributedCache;
    }
    public List<MenuItemModel> GetMenus()
    {
        return this._menuItemRepository.AsNoTracking().ToList<MenuItem,MenuItemModel>().Where(o=>o.ParentId==null).ToList();
    }
}
