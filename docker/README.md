# 启动前准备

*   安装 docker、docker-compose 环境

*   使用 vs 编译后台 Admin.NET 复制发布文件到 *docker/app/*

*   如果服务器有 node 环境使用 build.sh 编译前端文件到 *docker/nginx/dist*。或者将编译结果放进 *docker/nginx/dist*

# 注意事项

1.  *docker/app/Configuration/Database.json* 文件不需要修改，不要覆盖掉了
2.  *app/Configuration/App.json* 主要配置了 api 端口 5050，如果你的端口也是这个可以覆盖
2.  *Web/.env.production* 文件配置接口地址配置为 VITE\_API\_URL = '/prod-api'
3.  nginx、mysql 配置文件无需修改
4.  redis/redis.conf 中配置密码，如果不设密码 REDIS_PASSWORD 置空，app/Configuration/Cache.json中server=redis:6379，password 置空

***

# 启动命令

`docker-compose up -d`

# 访问入口

***<http://ip:9100>***
***<https://ip:9103>***
