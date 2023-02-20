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
├── Domain                          # 领域模型通用类型
├── Application                     # 应用服务通用类型
├── Resources                       # 通用资源文件
├── Extensions                      # 通用扩展方法
├── App.cs                          # 单例模式用于提供 IServiceProvider 访问
```

### WTA.Infrastructure 

业务无关类库

1. 三方依赖的接口实现

### WTA.Application.[Service]

按业务模块划分，不包含三方类库引用

```
├── Domain                          # 领域模型
├── Data                            # EFCore 实体配置和种子数据
├── Resources                       # 资源文件
├── Services                        # 应用服务跟目录
  ├── [Service]                     # 应用服务目录,定义服务的接口、实现和模型
├── Startup.cs                      # 服务配置，继承自 IStartup
├── [Service]DbContext              # 数据配置，继承自 IDbContext
```
