import { useContext, useEffect, useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { Button, Label, Select, TextInput } from "flowbite-react";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { AppContext } from "../apps/AppContext";
import { useAuth0 } from "@auth0/auth0-react";
import { toast } from "react-toastify";
import Route from "./Route";
import { useNavigate } from "react-router-dom";
import ILog from "../apps/Log";
import * as monaco from "monaco-editor/esm/vs/editor/editor.api";

type Inputs = {
  name: string;
  path: string;
  method: string;
  responseStatusCode: number;
  responseBodyLanguage: string;
  responseBody: string;
};

export default function RouteCreateUpdate({
  routeId,
  methods,
}: {
  routeId?: number;
  methods: string[];
}) {
  const isEditMode = routeId !== undefined;
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
  const schema = yup.object({
    name: yup.string().required(),
    path: yup.string().required(),
    method: yup.string().required(),
    responseStatusCode: yup.number().required(),
    responseBodyLanguage: yup.string().required(),
    responseBody: yup
      .string()
      .required()
      .max(400, "Response text must be at most 400 characters"),
  });
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
  } = useForm<Inputs>({
    resolver: yupResolver(schema),
  });
  const [logs, setLogs] = useState<ILog[]>([]);
  const [editor, setEditor] =
    useState<monaco.editor.IStandaloneCodeEditor | null>(null);
  const monacoEl = useRef(null);
  useEffect(() => {
    if (monacoEl) {
      setEditor((editor) => {
        if (editor) return editor;

        const instance = monaco.editor.create(monacoEl.current!, {
          language: "json",
        });

        instance.onDidChangeModelContent(() => {
          console.log("onDidChangeModelContent");
          setValue("responseBody", instance.getValue());
        });
        return instance;
      });
    }
    return () => editor?.dispose();
  }, [monacoEl.current]);
  useEffect(() => {
    if (!editor) {
      return;
    }
    const getRoute = async (id: number) => {
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
      setValue("responseStatusCode", route.responseStatusCode);
      setValue("responseBodyLanguage", route.responseBodyLanguage);
      editor.setValue(route.responseBody);
    };
    const getLogs = async (id: number) => {
      const accessToken = await getAccessTokenSilently();
      const logs = (await (
        await fetch(`/api/apps/${app.id}/logs?routeId=${id}`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        })
      ).json()) as ILog[];
      setLogs(logs);
    };
    if (isEditMode) {
      getRoute(routeId);
      getLogs(routeId);
    }
  }, [editor]);

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
  const bodyLanguages = ["json", "plaintext"];
  return (
    <div>
      <form className="p-2" onSubmit={handleSubmit(onSubmit)}>
        <div>
          <div className="mb-1 block">
            <Label htmlFor="name" value="Name" />
          </div>
          <TextInput
            sizing="sm"
            id="name"
            type="text"
            {...register("name")}
            autoComplete="none"
          />
          {errors.name && <span>{errors.name.message}</span>}
        </div>
        <h6 className="mt-3 border-l-2 border-cyan-600 pl-1">Request</h6>
        <div className="mt-2">
          <div className="mb-1 block">
            <Label htmlFor="path" value="Method and Path" />
          </div>
          <div className="flex">
            <Select
              sizing="sm"
              id="countries"
              className="w-24"
              {...register("method")}
            >
              {methods.map((m) => (
                <option key={m} value={m.toUpperCase()}>
                  {m}
                </option>
              ))}
            </Select>
            <TextInput
              id="path"
              type="text"
              className="w-full"
              sizing="sm"
              {...register("path")}
            />
          </div>
          {errors.method && <span>{errors.method.message}</span>}
          {errors.path && <span>{errors.path.message}</span>}
        </div>
        <h6 className="mt-3 border-l-2 border-cyan-600 pl-1">Response</h6>
        <div>
          <div className="mb-1 block">
            <Label htmlFor="responseStatusCode" value="Status Code" />
          </div>
          <TextInput
            sizing="sm"
            id="name"
            type="number"
            {...register("responseStatusCode")}
            autoComplete="none"
          />
          {errors.responseStatusCode && (
            <span>{errors.responseStatusCode.message}</span>
          )}
        </div>
        <div className="mb-5 mt-3">
          <label
            htmlFor="responseBody"
            className="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
          >
            Body
          </label>
          <div>
            <label htmlFor="responseBodyLanguage">Language</label>
            <select {...register("responseBodyLanguage")}>
              {bodyLanguages.map((l) => (
                <option key={l}>{l}</option>
              ))}
            </select>
          </div>
          <div style={{ height: "120px" }} className="border">
            <div ref={monacoEl} style={{ width: "80%", height: "100%" }}></div>
          </div>
          {errors.responseBody && (
            <p className="text-red-500">{errors.responseBody.message}</p>
          )}
        </div>
        <Button type="submit" size="sm" className="mt-2">
          Save
        </Button>
      </form>
      <div>
        <h1>Logs</h1>
        <ul>
          {logs.map((l) => (
            <li key={l.id}>
              <div className="text-sm">
                {new Date(l.timestamp).toUTCString()} {l.method} {l.path}
              </div>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
