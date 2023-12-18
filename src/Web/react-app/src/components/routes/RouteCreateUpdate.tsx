import { useEffect, useState } from "react";
import Route from "./Route";
import routes from "../../data/routes.json";
import { useForm } from "react-hook-form";
import { useParams } from "react-router-dom";
import { Button, Label, Select, TextInput } from "flowbite-react";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";

type Inputs = {
  name: string;
  path: string;
  method: string;
};

export default function RouteCreateUpdate() {
  console.log("RouteCreateUpdate");
  const routeId = useParams()["routeId"]
    ? parseInt(useParams()["routeId"]!.toString())
    : undefined;
  const isEditMode = routeId !== undefined;
  const [route, setRoute] = useState<Route>();

  useEffect(() => {
    if (isEditMode) {
      setRoute(routes.find((r) => r.id === routeId)!);
    }
  }, []);
  const schema = yup.object({
    name: yup.string().required(),
    path: yup.string().required(),
    method: yup.string().required(),
  });
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Inputs>({
    resolver: yupResolver(schema),
  });
  const onSubmit = (data: Inputs) => {
    console.log(data);
  };
  return (
    <>
      <form
        className="flex max-w-md flex-col gap-4"
        onSubmit={handleSubmit(onSubmit)}
      >
        <div>
          <div className="mb-2 block">
            <Label htmlFor="name" value="Name" />
          </div>
          <TextInput sizing="sm" id="name" type="text" {...register("name")} />
          {errors.name && <span>{errors.name.message}</span>}
        </div>
        <div>
          <div className="mb-2 block">
            <Label htmlFor="path" value="Method and Path" />
          </div>
          <div>
            <Select id="countries" className="col-span-1">
              <option>United States</option>
              <option>Canada</option>
              <option>France</option>
              <option>Germany</option>
            </Select>
            <TextInput id="path" type="text" {...register("path")} />
          </div>
          {errors.method && <span>{errors.method.message}</span>}
          {errors.path && <span>{errors.path.message}</span>}
        </div>
        <Button type="submit">Submit</Button>
      </form>
    </>
  );
}
