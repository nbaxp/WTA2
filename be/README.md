# 说明

## 目录结构

```
├── README.md                       # 说明
├── src
│   ├── WTA.Application             # 应用服务共享类库
│   ├── WTA.Infrastructure          # 基础设施类库
│   ├── WTA.Services                # 应用服务项目
│       ├── WTA.Services.Identity   # 身份认证服务
│       ├── WTA.Services.BaseData   # 基础数据服务
│       ├── WTA.Services.Wms        # 仓储管理服务
│   ├── WTA.Web                     # Web API 项目，业务无关
│   ├── WTA.Migrate                 # 数据库初始化项目，业务无关
```

### WTA.Application

业务无关类库，不包含三方类库引用

1. 领域模型通用类型定义
1. 应用服务通用类型定义
1. 应用服务的三方依赖接口、参数和返回值的类型定义

```
├── Abstractions                    # 接口、模型和配置
├── Application                     # 应用服务通用类型
├── Domain                          # 领域模型通用类型
├── Extensions                      # 通用扩展方法
├── Resources                       # 通用资源文件
├── Views                           # 通用视图
├── App.cs                          # 单例模式用于提供 IServiceProvider 访问
```

### WTA.Infrastructure 

业务无关类库，三方依赖的接口实现

```
├── Authentication                  # ITokenService 认证接口实现
├── Data                            # IRepository<T> 接口实现
├── DataAnnotations                 # DataAnnotations 验证配置
├── Extensions                      # 扩展方法
├── GuidGenerators                  # IGuidGenerator 接口实现
├── Identity                        # IPasswordHasher 接口实现
├── GuidGenerators                  # IGuidGenerator 接口实现
├── Localization                    # JSON 文件
├── Mappers                         # IObjectMapper 接口实现
├── Options                         # wwwroot 静态文件嵌入程序集
├── Routing                         # IOutboundParameterTransformer 接口连字符格式实现
├── Swagger                         # Swashbuckle 配置
├── Tenants                         # ITenantService 接口实现
├── WebApp.cs                       # Web 项目启动器
```

### WTA.Application.[Service]

按业务模块划分，不包含三方类库引用

```
├── Domain                          # 领域模型
├── Services                        # 应用服务跟目录
│   ├── [Service]                   # 应用服务目录,定义服务的接口、实现和模型
├── Data                            # EFCore 实体配置和种子数据
│   ├── [Service]DbContext          # 数据库上下文配置，继承自 IDbContext
│   ├── [Service]Configuration      # 实体配置，继承自 IEntityTypeConfiguration
├── Resources                       # 资源文件
├── Startup.cs                      # 服务配置，继承自 IStartup
```

#### WTA.Application.Identity

身份认证模块

权限生成规则：
1.分组：ModuleAttribute->GroupAttribute
