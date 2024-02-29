export default interface ILog {
  id: string;
  timestamp: string;
  remoteIp: string;
  appId: number;
  routeId?: number;
  routeName?: string;
  method: string;
  path: string;
  statusCode: number;
  additionalLogMessage?: string;
  functionExecutionDuration?: number;
}
