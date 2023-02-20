# 说明

## 目录结构

```
├── README.md						# 说明
├── src
│   ├── WTA.Core                    # 全局共享类库，业务无关
│   ├── WTA.Application             # 应用服务共享类库，业务无关
│   ├── WTA.Services                # 应用服务项目，按业务模块划分
│   ├── WTA.Web                     # Web API 项目，业务无关
│   ├── WTA.Infrastructure          # 基础设施类库，业务无关
│   ├── WTA.Migrate			        # 数据库初始化项目，业务无关
```

### WTA.Core

不引用其他程序集

```
├── App.cs							# 单例模式用于提供 IServiceProvider 访问
├── Extensions						# 通用扩展方法
├── Domain							# 领域模型通用类型
├── Application						# 应用服务通用类型
```

### WTA.Application     

为便于快速开发，引用 Microsoft.AspNetCore.App 和 Microsoft.EntityFrameworkCore.Abstractions

```
├── App.cs							# 单例模式用于提供 IServiceProvider 访问
├── Extensions						# 通用扩展方法
├── Domain							# 领域模型通用类型
├── Application						# 应用服务通用类型
```

### WTA.Application.Services

1. 不引用任何类库，按业务模块化划分，每个模块一个项目
1. Service 同时作为 Controller,不以 Controller 为基类，使用构造函数注入 IHttpContextAccessor 的方法访问 HttpContext
1. 每个 Service 一个目录，包含模型和服务

```
├── Domain							# 领域模型
├── Data							# EF 相关配置和种子数据
├── Services						# 应用服务
```
