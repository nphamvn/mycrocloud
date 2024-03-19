import * as yup from "yup";
import type { ObjectSchema } from "yup";
import { methods } from "./constants";

export type RouteCreateUpdateInputs = {
  name: string;
  path: string;
  method: string;
  requireAuthorization: boolean;
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
  useDynamicResponse?: boolean;
  fileId?: number;
};

export interface HeaderInput {
  name: string;
  value: string;
}

export const routeCreateUpdateInputsSchema: ObjectSchema<RouteCreateUpdateInputs> =
  yup.object({
    name: yup.string().required("Name is required"),
    path: yup.string().required().matches(/^\//, "Path must start with /"),
    method: yup.string().required().oneOf(methods),
    requireAuthorization: yup.boolean().required(),
    requestQuerySchema: yup.string().defined(),
    requestHeaderSchema: yup.string().defined(),
    requestBodySchema: yup.string().defined(),
    responseType: yup.string().required(),
    responseStatusCode: yup.number().defined(),
    responseHeaders: yup.array().of(
      yup.object({
        name: yup.string().required(),
        value: yup.string().required(),
      }),
    ),
    responseBodyLanguage: yup.string().defined(),
    responseBody: yup.string().defined(),
    functionHandler: yup.string().defined(),
    functionHandlerDependencies: yup.array().of(yup.string().required()),
    useDynamicResponse: yup.boolean().defined(),
    fileId: yup.number().defined(),
  });
