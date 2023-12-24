export default interface ILog {
  id: string;
  timestamp: string;
  appId: number;
  routeId?: number;
  method: string;
  path: string;
  statusCode: number;
}
