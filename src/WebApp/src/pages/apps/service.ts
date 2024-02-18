export function getAppDomain(appId: number) {
  const apiGatewayDomain = import.meta.env
    .VITE_WEBAPP_APIGATEWAY_DOMAIN as string;
  return apiGatewayDomain.replace("__app_id__", appId.toString());
}
