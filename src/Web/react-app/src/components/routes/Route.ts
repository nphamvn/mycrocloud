export default interface Route {
  id: number | undefined;
  name: string;
  path: string;
  method: string;
  responseStatusCode: number;
  responseBodyLanguage: string;
  responseBody: string;
}
