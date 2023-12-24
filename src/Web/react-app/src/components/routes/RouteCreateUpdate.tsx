import { useContext, useEffect } from "react";
import routes from "../../data/routes.json";
import { useForm } from "react-hook-form";
import { Button, Label, Select, TextInput } from "flowbite-react";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { AppContext } from "../apps/AppContext";
import { useAuth0 } from "@auth0/auth0-react";
import { toast } from "react-toastify";
import Route from "./Route";

type Inputs = {
  name: string;
  path: string;
  method: string;
  responseText: string;
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
  const schema = yup.object({
    name: yup.string().required(),
    path: yup.string().required(),
    method: yup.string().required(),
    responseText: yup
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
  useEffect(() => {
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
      setValue("responseText", route.responseText);
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
    }
  };
  return (
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
      <div className="mb-5 mt-3">
        <label
          htmlFor="responseText"
          className="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
        >
          Response Text
        </label>
        <textarea
          id="responseText"
          rows={4}
          {...register("responseText")}
          spellCheck={false}
          className="block w-full border border-gray-300 bg-gray-50 text-sm text-gray-900 focus:border-cyan-500 focus:ring-cyan-500 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-cyan-500 dark:focus:ring-cyan-500"
        ></textarea>
        {errors.responseText && (
          <p className="text-red-500">{errors.responseText.message}</p>
        )}
      </div>
      <Button type="submit" size="sm" className="mt-2">
        Save
      </Button>
    </form>
  );
}
