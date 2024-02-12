import * as yup from "yup";

export type RouteCreateUpdateInputs = {
  name: string;
  path: string;
  method: string;
  responseType: string;
  responseStatusCode?: number;
  responseHeaders?: HeaderInput[];
  responseBodyLanguage?: string;
  responseBody?: string;
  functionHandler?: string;
  functionHandlerDependencies?: string[];
  requireAuthorization: boolean;
  staticFile?: File;
  useDynamicResponse?: boolean;
};

export interface HeaderInput {
  name: string;
  value: string;
}

export const routeCreateUpdateInputsSchema = yup.object({
  name: yup.string().required(),
  path: yup.string().required(),
  method: yup.string().required(),
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
  staticFile: yup.mixed(),
  useDynamicResponse: yup.boolean(),
});
