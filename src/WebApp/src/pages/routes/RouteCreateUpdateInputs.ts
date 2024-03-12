import * as yup from "yup";

export type RouteCreateUpdateInputs = {
  name: string;
  path: string;
  method: string;
  requestQuerySchema?: string;
  requestHeaderSchema?: string;
  requestBodySchema?: string;
  responseType: string;
  responseStatusCode?: number;
  responseHeaders?: HeaderInput[];
  responseBodyLanguage?: string;
  responseBody?: string;
  functionHandler?: string;
  functionHandlerDependencies?: string[];
  requireAuthorization: boolean;
  useDynamicResponse?: boolean;
};

export interface HeaderInput {
  name: string;
  value: string;
}

export const routeCreateUpdateInputsSchema = yup.object({
  name: yup.string().required("Name is required"),
  path: yup.string().required().matches(/^\//, "Path must start with /"),
  method: yup.string().required(),
  requestQuerySchema: yup.string(),
  requestHeaderSchema: yup.string(),
  requestBodySchema: yup.string(),
  responseType: yup.string().required(),
  responseStatusCode: yup.number(),
  responseHeaders: yup.array().of(
    yup.object({
      name: yup.string().required(),
      value: yup.string().required(),
    }),
  ),
  responseBodyLanguage: yup.string(),
  responseBody: yup.string(),
  functionHandler: yup.string(),
  functionHandlerDependencies: yup.array().of(yup.string().required()),
  requireAuthorization: yup.boolean().required(),
  useDynamicResponse: yup.boolean(),
});
