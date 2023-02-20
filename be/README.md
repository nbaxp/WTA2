# 说明

## 目录结构

```
├── README.md                       # 说明
├── src                             
│   ├── WTA.Core                    # 全局共享类库
│   ├── WTA.Application             # 应用服务共享类库
│   ├── WTA.Infrastructure          # 基础设施类库
│   ├── WTA.Services                # 应用服务项目
│   ├── WTA.Web                     # Web API 项目，业务无关
│   ├── WTA.Migrate                 # 数据库初始化项目，业务无关
```

### WTA.Core

业务无关，主要包含领域模型基础类型，不包含三方类库引用

```
├── App.cs                          # 单例模式用于提供全局 IServiceProvider 访问
├── Extensions                      # 通用扩展方法
├── Domain                          # 领域模型通用类型
├── Application                     # 应用服务通用类型
```

### WTA.Application     

业务无关，主要包含应用服务的三方依赖接口定义

```
├── App.cs                          # 单例模式用于提供 IServiceProvider 访问
├── Extensions                      # 通用扩展方法
├── Domain                          # 领域模型通用类型
├── Application                     # 应用服务通用类型
```

### WTA.Infrastructure 

业务无关，主要包含三方依赖的接口实现

### WTA.Application.Services

按业务模块划分，不包含三方类库引用

```
├── Domain                          # 领域模型
├── Data                            # EF 相关配置和种子数据
├── Services                        # 应用服务
```
