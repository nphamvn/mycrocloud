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
  requestQuerySchema: yup.string().nullable(),
  requestHeaderSchema: yup.string().nullable(),
  requestBodySchema: yup.string().nullable(),
  responseType: yup.string().required(),
  responseStatusCode: yup.number().nullable(),
  responseHeaders: yup
    .array()
    .of(
      yup.object({
        name: yup.string().required(),
        value: yup.string().required(),
      }),
    )
    .optional(),
  responseBodyLanguage: yup.string().nullable(),
  responseBody: yup.string().nullable(),
  functionHandler: yup.string().nullable(),
  functionHandlerDependencies: yup
    .array()
    .of(yup.string().required())
    .optional(),
  requireAuthorization: yup.boolean().required(),
  useDynamicResponse: yup.boolean().nullable(),
});
