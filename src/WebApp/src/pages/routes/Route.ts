export default interface IRoute {
  id: number;
  name: string;
  path: string;
  method: string;
  responseType: string;
  responseStatusCode?: number;
  responseHeaders?: IResponseHeader[];
  responseBodyLanguage?: string;
  responseBody?: string;
  functionHandler?: string;
  functionHandlerDependencies?: string[];
  requireAuthorization: boolean;
  status: string;
  useDynamicResponse: boolean;
}

export interface IResponseHeader {
  name: string;
  value: string;
}
