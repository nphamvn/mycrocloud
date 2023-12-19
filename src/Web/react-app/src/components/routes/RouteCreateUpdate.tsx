import { useEffect, useState } from "react";
import routes from "../../data/routes.json";
import { useForm } from "react-hook-form";
import { Button, Label, Select, TextInput } from "flowbite-react";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";

type Inputs = {
  name: string;
  path: string;
  method: string;
};

export default function RouteCreateUpdate({
  routeId,
  methods,
}: {
  routeId?: number;
  methods: string[];
}) {
  const isEditMode = routeId !== undefined;
  const schema = yup.object({
    name: yup.string().required(),
    path: yup.string().required(),
    method: yup.string().required(),
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
    if (isEditMode) {
      const route = routes.find((r) => r.id === routeId)!;
      setValue("name", route.name);
      setValue("method", route.method.toUpperCase());
      setValue("path", route.path);
    }
  }, []);

  const onSubmit = (data: Inputs) => {
    console.log(data);
  };
  return (
    <form className="p-2" onSubmit={handleSubmit(onSubmit)}>
      <div>
        <div className="mb-1 block">
          <Label htmlFor="name" value="Name" />
        </div>
        <TextInput sizing="sm" id="name" type="text" {...register("name")} />
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
            className="col-span-1"
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
      <Button type="submit" size="sm" className="mt-2">
        Save
      </Button>
    </form>
  );
}
