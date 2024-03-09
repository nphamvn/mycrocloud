import { IRoute } from ".";

export const methods = ["get", "post", "put", "delete", "patch"];
export const responseBodyRenderTemplateEngines = ["handlebars", "mustache"];
export const bodyLanguages = [
  "json",
  "form",
  "plaintext",
  "html",
  "javascript",
  "xml",
  "yaml",
  "markdown",
  "css",
  "handlebars",
];

export const sampleRoute: IRoute = {
  id: 0,
  name: "untitled",
  method: "GET",
  path: "",
  status: "Active",
  responseType: "static",
  useDynamicResponse: false,
  requireAuthorization: false,
};
