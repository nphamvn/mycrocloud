export default interface Route {
  id: number;
  name: string;
  path: string;
  method: string;
  responseType: string;
  responseStatusCode?: number;
  responseHeaders?: Header[];
  responseBodyLanguage?: string;
  responseBody?: string;
  functionHandler?: string;
  functionHandlerDependencies?: string[];
  requireAuthorization: boolean;
  status: string;
}

interface Header {
  name: string;
  value: string;
}
