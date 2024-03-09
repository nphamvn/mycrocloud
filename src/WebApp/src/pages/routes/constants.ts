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
  status: "Active",
  name: "untitled",
  method: "GET",
  path: "/",
  requireAuthorization: false,
  responseType: "static",
  responseStatusCode: 200,
  responseHeaders: [{ name: "content-type", value: "text/plain" }],
  useDynamicResponse: false,
  responseBodyLanguage: "plaintext",
};
