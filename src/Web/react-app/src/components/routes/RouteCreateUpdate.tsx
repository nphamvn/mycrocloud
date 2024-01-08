import { useContext, useEffect, useRef, useState } from "react";
import {
  FormProvider,
  useFieldArray,
  useForm,
  useFormContext,
} from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { AppContext } from "../apps/AppContext";
import { useAuth0 } from "@auth0/auth0-react";
import { toast } from "react-toastify";
import Route from "./Route";
import { useNavigate } from "react-router-dom";
import * as monaco from "monaco-editor/esm/vs/editor/editor.api";
import { bodyLanguages, methods } from "./constants";

type Inputs = {
  name: string;
  path: string;
  method: string;
  responseType: string;
  responseStatusCode?: number;
  responseHeaders?: HeaderInput[];
  responseBodyLanguage?: string;
  responseBody?: string;
  functionHandler?: string;
};

interface HeaderInput {
  name: string;
  value: string;
}

const schema = yup.object({
  name: yup.string().required(),
  path: yup.string().required(),
  method: yup.string().required(),
  responseType: yup.string().required(),
});

export default function RouteCreateUpdate({ routeId }: { routeId?: number }) {
  const isEditMode = routeId !== undefined;
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const forms = useForm<Inputs>({
    resolver: yupResolver(schema),
    defaultValues: {
      responseType: "static",
      responseStatusCode: 200,
      responseHeaders: [{ name: "content-type", value: "text/plain" }],
      responseBodyLanguage: "plaintext",
    },
  });
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    watch,
  } = forms;
  const responseType = watch("responseType");

  useEffect(() => {
    const getRoute = async (id: number) => {
      setIsLoading(true);
      const accessToken = await getAccessTokenSilently();
      const route = (await (
        await fetch(`/api/apps/${app.id}/routes/${id}`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        })
      ).json()) as Route;
      setValue("name", route.name);
      setValue("method", route.method.toUpperCase());
      setValue("path", route.path);
      setValue("responseType", route.responseType);
      if (route.responseType === "static") {
        setValue("responseStatusCode", route.responseStatusCode!);
        setValue(
          "responseHeaders",
          route.responseHeaders!.map((h) => {
            return {
              name: h.name,
              value: h.value,
            };
          }),
        );
        setValue("responseBodyLanguage", route.responseBodyLanguage!);
        setValue("responseBody", route.responseBody!);
      } else {
        setValue("functionHandler", route.functionHandler!);
      }
      setIsLoading(false);
    };
    if (isEditMode) {
      getRoute(routeId);
    }
  }, []);

  const onSubmit = async (data: Inputs) => {
    const accessToken = await getAccessTokenSilently();
    const res = await fetch(
      isEditMode
        ? `/api/apps/${app.id}/routes/${routeId}`
        : `/api/apps/${app.id}/routes`,
      {
        method: isEditMode ? "PUT" : "POST",
        headers: {
          "content-type": "application/json",
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify(data),
      },
    );
    if (res.ok) {
      toast("Created route");
      const routeId = 1;
      if (!isEditMode) {
        navigate(`apps/${app.id}/routes/${routeId}`);
      }
    }
  };
  if (isLoading) {
    return <div className="w-full">Loading...</div>;
  }
  return (
    <form className="h-full p-2" onSubmit={handleSubmit(onSubmit)}>
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
          {errors.name && <span>{errors.name.message}</span>}
        </div>
        <section>
          <h3 className="border-primary mt-3 border-l-2 px-1 font-semibold">
            Request
          </h3>
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
            {errors.method && <span>{errors.method.message}</span>}
            {errors.path && <span>{errors.path.message}</span>}
          </div>
        </section>
        <section>
          <h3 className="border-primary mt-3 border-l-2 pl-1 font-semibold">
            Response
          </h3>
          <div className="mt-1">
            <label className="me-1">Type</label>
            <select {...register("responseType")}>
              <option value="static">static</option>
              <option value="function">function</option>
            </select>
          </div>
          <FormProvider {...forms}>
            <div>
              {responseType === "static" && <StaticResponse />}
              {responseType === "function" && <FunctionHandler />}
            </div>
          </FormProvider>
        </section>
      </div>
      <div className="sticky bottom-0 mt-2">
        <button
          type="submit"
          className="border border-transparent bg-cyan-700 px-3 py-1 text-center font-medium text-white focus:z-10 focus:outline-none focus:ring-2 focus:ring-cyan-300 enabled:hover:bg-cyan-800"
        >
          Save
        </button>
      </div>
    </form>
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
  } = useFormContext<Inputs>();
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
    let isMounted = true;
    if (bodyEditorRef.current && isMounted) {
      setBodyEditor((editor) => {
        if (editor) return editor;
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
    }

    return () => {
      isMounted = false;
    };
  }, [bodyEditorRef.current]);
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
        <div className="flex">
          <label className="block">Body</label>
          <div className="ms-auto">
            <label htmlFor="responseBodyLanguage">Editor format</label>
            <select {...register("responseBodyLanguage")}>
              {bodyLanguages.map((l) => (
                <option key={l}>{l}</option>
              ))}
            </select>
          </div>
        </div>
        <div>
          <div
            ref={bodyEditorRef}
            style={{ width: "100%", height: "120px" }}
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
  } = useFormContext<Inputs>();
  const handlerEditorRef = useRef(null);
  const [, setHandlerEditor] = useState<monaco.editor.IStandaloneCodeEditor>();

  useEffect(() => {
    let isMounted = true;
    if (handlerEditorRef.current && isMounted) {
      setHandlerEditor((editor) => {
        if (editor) return editor;
        const instance = monaco.editor.create(handlerEditorRef.current!, {
          language: "javascript",
          value:
            getValues("functionHandler") || `function handler(req, res) {\n}`,
          minimap: {
            enabled: false,
          },
        });
        instance.onDidChangeModelContent(() => {
          setValue("functionHandler", instance.getValue());
        });
        return instance;
      });
    }

    return () => {
      isMounted = false;
    };
  }, [handlerEditorRef.current]);

  return (
    <div className="mt-1">
      <label>Handler</label>
      <div
        ref={handlerEditorRef}
        style={{ width: "100%", height: "600px" }}
      ></div>
      {errors.functionHandler && (
        <p className="text-red-500">{errors.functionHandler.message}</p>
      )}
    </div>
  );
}
