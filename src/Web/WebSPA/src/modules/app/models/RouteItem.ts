export default interface RouteItem {
  id: number;
  name: string;
  method: string;
  path: string;
  desciption: string;
  responseType: string;
  response?: MockResponse;
}
export interface MockResponse {
  statusCode: number;
  body: string;
  bodyLanguage: string;
}
