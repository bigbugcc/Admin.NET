#!/bin/sh
# 红色信息
function echoRedInfo() {
    echo -e "\e[31m$@\e[0m"
}
# 绿色信息
function echoGreenInfo() {
    echo -e "\e[32m$@\e[0m"
}
# 蓝色信息
function echoBlueInfo() {
    echo -e "\e[34m$@\e[0m"
}
url1="http://172.18.32.33:5050"
url2="http://127.0.0.1:5050"
url3="http://localhost:5005"
# 打印菜单
echoBlueInfo "请选择Swagger地址:"
echoBlueInfo "(1) $url1"
echoBlueInfo "(2) $url2"
echoBlueInfo "(3) $url3"
read -p "请输入选项 [1-3]: " choice

currPath=$(pwd)
parentPath=$(dirname "$currPath")
apiServicesPath=${parentPath}/src/api-services/

echo "生成目录 ${apiServicesPath}"

# 判断目录是否存在
if test -d "$apiServicesPath"; then
  echo "删除目录 api-services"
  rm -rf "${apiServicesPath}"
fi

echo "开始生成 api-services"


# 检查用户输入并执行相应操作
case $choice in
    1)
        echoGreenInfo "您选择了: $url1"
        java -jar "${currPath}"/swagger-codegen-cli.jar generate -i $url1/swagger/All%20Groups/swagger.json -l typescript-axios -o "${apiServicesPath}"
        ;;
    2)
        echoGreenInfo "您选择了: $url2"
        java -jar "${currPath}"/swagger-codegen-cli.jar generate -i $url2/swagger/All%20Groups/swagger.json -l typescript-axios -o "${apiServicesPath}"
        ;;
    3)
        echoGreenInfo "您选择了: $url3"
        java -jar "${currPath}"/swagger-codegen-cli.jar generate -i $url3/swagger/All%20Groups/swagger.json -l typescript-axios -o "${apiServicesPath}"
        ;;
    *)
        echoRedInfo "无效的选项，请输入[1-3]。"
        exit 1
        ;;
esac
rm -rf "${apiServicesPath}".swagger-codegen
rm -f "${apiServicesPath}".gitignore
rm -f "${apiServicesPath}".npmignore
rm -f "${apiServicesPath}".swagger-codegen-ignore
rm -f "${apiServicesPath}"git_push.sh
rm -f "${apiServicesPath}"package.json
rm -f "${apiServicesPath}"README.md
rm -f "${apiServicesPath}"tsconfig.json

echoGreenInfo "生成结束"
