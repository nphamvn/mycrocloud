import { useContext, useEffect, useRef, useState } from "react";
import {
  FormProvider,
  useFieldArray,
  useForm,
  useFormContext,
} from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import { AppContext } from "../apps";
import IRoute from "./Route";
import * as monaco from "monaco-editor/esm/vs/editor/editor.api";
import { bodyLanguages, methods } from "./constants";
import {
  RouteCreateUpdateInputs,
  routeCreateUpdateInputsSchema,
} from "./RouteCreateUpdateInputs";
const apiGatewayDomain = import.meta.env
  .VITE_WEBAPP_APIGATEWAY_DOMAIN as string;

export default function RouteCreateUpdate({
  route,
  onSubmit,
}: {
  route?: IRoute;
  onSubmit: (data: RouteCreateUpdateInputs) => void;
}) {
  const app = useContext(AppContext)!;
  const appDomain = apiGatewayDomain.replace("__app_id__", app.id.toString());

  const forms = useForm<RouteCreateUpdateInputs>({
    resolver: yupResolver(routeCreateUpdateInputsSchema),
    defaultValues: {
      name: route?.name || "",
      method: route?.method || "GET",
      path: route?.path || "",
      requireAuthorization: route?.requireAuthorization || false,
      responseType: route?.responseType || "static",
      responseStatusCode: route?.responseStatusCode || 200,
      responseHeaders: route?.responseHeaders
        ? route.responseHeaders.map((value) => {
            return {
              name: value.name,
              value: value.value,
            };
          })
        : [],
      responseBody: route?.responseBody || "",
      responseBodyLanguage: route?.responseBodyLanguage || "json",
      functionHandler: route?.functionHandler || "",
      functionHandlerDependencies: route?.functionHandlerDependencies || [],
      useDynamicResponse: route?.useDynamicResponse || false,
    },
  });
  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = forms;

  const responseType = watch("responseType");
  const url = appDomain + watch("path");

  return (
    <FormProvider {...forms}>
      <form className="h-full p-2" onSubmit={handleSubmit(onSubmit)}>
        {route?.status === "Blocked" && (
          <div className="border border-red-200 bg-red-50 p-2 text-red-700">
            <p>
              This route is blocked because of some reason. Your route will be
              reviewed by our team.
            </p>
          </div>
        )}
        <div className="overflow-y-auto">
          <div>
            <label htmlFor="name">Name</label>
            <input
              id="name"
              type="text"
              {...register("name")}
              autoComplete="none"
              className="inline-block w-full border border-gray-200 px-2 py-1"
            />
            {errors.name && (
              <span className="text-red-500">{errors.name.message}</span>
            )}
          </div>
          <section>
            <div className="flex">
              <h3 className="mt-3 border-l-2 border-primary px-1 font-semibold">
                Request
              </h3>
            </div>
            <div className="mt-2">
              <label>Method and Path</label>
              <div className="flex">
                <select
                  className="w-24 border border-gray-200"
                  {...register("method")}
                >
                  {methods.map((m) => (
                    <option key={m} value={m.toUpperCase()}>
                      {m.toUpperCase()}
                    </option>
                  ))}
                </select>
                <input
                  autoComplete="none"
                  type="text"
                  className="inline-block flex-1 border border-gray-200 px-2 py-1"
                  {...register("path")}
                />
              </div>
              {errors.method && (
                <span className="text-red-500">{errors.method.message}</span>
              )}
              {errors.path && (
                <span className="text-red-500">{errors.path.message}</span>
              )}
              <div className="mt-1">
                <small className="me-2">URL:</small>
                <a className="text-blue-500 hover:underline" href={url}>
                  <small>{url}</small>
                </a>
                <button
                  type="button"
                  onClick={() => navigator.clipboard.writeText(url)}
                  className="ms-3 text-blue-500 hover:underline"
                >
                  <small>Copy</small>
                </button>
              </div>
            </div>
            <div className="mt-1">
              <div>
                <input
                  type="checkbox"
                  {...register("requireAuthorization")}
                  className="inline-block border border-gray-200 px-2 py-1"
                />
                <label className="mt-2">Require Authorization</label>
              </div>
              {errors.requireAuthorization && (
                <span className="text-red-500">
                  {errors.requireAuthorization.message}
                </span>
              )}
            </div>
          </section>
          <section>
            <h3 className="mt-3 border-l-2 border-primary pl-1 font-semibold">
              Response
            </h3>
            <div className="mt-1">
              <label className="me-1">Type</label>
              <select {...register("responseType")}>
                <option value="static">static</option>
                <option value="function">function</option>
              </select>
            </div>

            <div className="mt-1">
              {responseType === "static" && <StaticResponse />}
              {responseType === "function" && <FunctionHandler />}
            </div>
          </section>
        </div>
        <div className="sticky bottom-0 mt-2">
          <button
            type="submit"
            className="border border-transparent bg-cyan-700 px-3 py-1 text-center font-medium text-white focus:z-10 focus:outline-none focus:ring-2 focus:ring-cyan-300 enabled:hover:bg-cyan-800"
            disabled={route?.status === "Blocked"}
          >
            Save
          </button>
        </div>
      </form>
    </FormProvider>
  );
}

function StaticResponse() {
  const {
    control,
    register,
    formState: { errors },
    setValue,
    getValues,
    watch,
  } = useFormContext<RouteCreateUpdateInputs>();
  const {
    fields: responseHeaders,
    append: addResponseHeaders,
    remove: removeResponseHeader,
  } = useFieldArray({
    control,
    name: "responseHeaders",
  });

  const bodyEditorRef = useRef(null);
  const [bodyEditor, setBodyEditor] =
    useState<monaco.editor.IStandaloneCodeEditor>();

  useEffect(() => {
    setBodyEditor((prevEditor) => {
      if (prevEditor) return prevEditor;

      const instance = monaco.editor.create(bodyEditorRef.current!, {
        language: getValues("responseBodyLanguage"),
        value: getValues("responseBody"),
        minimap: {
          enabled: false,
        },
      });
      instance.onDidChangeModelContent(() => {
        setValue("responseBody", instance.getValue());
      });
      return instance;
    });

    return () => {
      bodyEditor?.dispose();
    };
  }, []);

  const responseBodyLanguage = watch("responseBodyLanguage");
  useEffect(() => {
    if (bodyEditor && responseBodyLanguage) {
      monaco.editor.setModelLanguage(
        bodyEditor.getModel()!,
        responseBodyLanguage,
      );
    }
  }, [responseBodyLanguage]);
  return (
    <>
      <div className="mt-2">
        <label htmlFor="responseStatusCode" className="block">
          Status Code
        </label>
        <input
          id="responseStatusCode"
          type="number"
          {...register("responseStatusCode")}
          autoComplete="none"
          className="w-1/6 border border-gray-200 px-2 py-1"
        />
        {errors.responseStatusCode && (
          <span>{errors.responseStatusCode.message}</span>
        )}
      </div>
      <div className="mt-2">
        <label htmlFor="header">Headers</label>
        <div className="flex flex-col space-y-0.5">
          {responseHeaders.map((header, index) => (
            <div key={header.id} className="flex space-x-1">
              <input
                id={`responseHeaders[${index}].name`}
                type="text"
                {...register(`responseHeaders.${index}.name` as const)}
                autoComplete="none"
                className="border border-gray-200 px-2 py-1"
              />
              <input
                id={`responseHeaders[${index}].value`}
                type="text"
                {...register(`responseHeaders.${index}.value` as const)}
                autoComplete="none"
                className="border border-gray-200 px-2 py-1"
              />
              <button
                type="button"
                onClick={() => removeResponseHeader(index)}
                className="text-red-600"
              >
                Remove
              </button>
            </div>
          ))}
        </div>
        <button
          type="button"
          onClick={() => addResponseHeaders({ name: "", value: "" })}
          className=" mt-1 text-blue-600"
        >
          Add
        </button>
      </div>
      <div className="mt-2">
        <label className="block">Body</label>
        <div className="mt-1 flex">
          <div>
            <input
              type="checkbox"
              {...register("useDynamicResponse")}
              className="inline-block border border-gray-200 px-2 py-1"
            />
            <label className="mt-2">Use dynamic response</label>
          </div>
          <div className="ms-auto">
            <label htmlFor="responseBodyLanguage">Editor format</label>
            <select {...register("responseBodyLanguage")}>
              {bodyLanguages.map((l) => (
                <option key={l}>{l}</option>
              ))}
            </select>
          </div>
        </div>
        <div className="mt-1">
          <div
            ref={bodyEditorRef}
            style={{ width: "100%", height: "300px" }}
          ></div>
          {errors.responseBody && (
            <p className="text-red-500">{errors.responseBody.message}</p>
          )}
        </div>
      </div>
    </>
  );
}

function FunctionHandler() {
  const {
    formState: { errors },
    setValue,
    getValues,
  } = useFormContext<RouteCreateUpdateInputs>();
  const handlerEditorRef = useRef(null);
  const [handlerEditor, setHandlerEditor] =
    useState<monaco.editor.IStandaloneCodeEditor>();

  useEffect(() => {
    if (handlerEditorRef) {
      setHandlerEditor((prev) => {
        if (prev) return prev;

        const editor = monaco.editor.create(handlerEditorRef.current!, {
          language: "javascript",
          value: getValues("functionHandler") || `function handler(req) {\n}`,
          minimap: {
            enabled: false,
          },
        });
        editor.onDidChangeModelContent(() => {
          setValue("functionHandler", editor.getValue());
        });
        return editor;
      });
    }

    return () => {
      handlerEditor?.dispose();
    };
  }, [handlerEditorRef.current]);

  const [depsInputValue, setDepsInputValue] = useState(
    getValues("functionHandlerDependencies")?.join(",") || "",
  );
  useEffect(() => {
    setValue("functionHandlerDependencies", depsInputValue?.split(",") || []);
  }, [depsInputValue]);

  return (
    <>
      <div className="mt-1">
        <label htmlFor="depsInput" className="block">
          Dependencies
          <div className="inline-block">
            <button
              type="button"
              className="ms-2 text-blue-500 hover:underline"
            >
              info
            </button>
            <div style={{ display: "none" }}>
              <p>Available packages</p>
              <ul>
                {[
                  "lotash",
                  "underscore",
                  "handlebars",
                  "mustache",
                  "faker",
                ].map((p) => (
                  <li key={p}>{p}</li>
                ))}
              </ul>
            </div>
          </div>
        </label>
        <input
          id="depsInput"
          type="text"
          value={depsInputValue}
          onChange={(e) => setDepsInputValue(e.target.value)}
          autoComplete="none"
          className="inline-block w-full border border-gray-200 px-2 py-1"
        />
      </div>
      <div className="mt-1">
        <label>Handler</label>
        <div
          ref={handlerEditorRef}
          style={{ width: "100%", height: "200px" }}
        ></div>
        {errors.functionHandler && (
          <p className="text-red-500">{errors.functionHandler.message}</p>
        )}
      </div>
    </>
  );
}