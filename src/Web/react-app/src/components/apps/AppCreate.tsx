import { SubmitHandler, useForm } from "react-hook-form";
import AppConfig from "../../constants/AppConfig";
import { useAuth } from "../../hooks/useAuth";
import { toast } from "react-toastify";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import { Button } from "flowbite-react";
import { useNavigate } from "react-router-dom";

type Inputs = {
  name: string;
};

function AppCreate() {
  const { user } = useAuth()!;
  const navigate = useNavigate();
  const schema = yup.object({
    name: yup.string().required(),
  });
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Inputs>({
    resolver: yupResolver(schema),
  });
  const onSubmit: SubmitHandler<Inputs> = async (data) => {
    try {
      toast(data.name);
      await fetch(`${AppConfig.BASE_API_URL}/api/apps/create`, {
        method: "POST",
        headers: {
          Authorzation: `Bearer ${user?.accessToken}`,
        },
      });
    } catch (error) {
      console.log(error);
    }
  };
  return (
    <form className="mx-auto mt-5 max-w-4xl" onSubmit={handleSubmit(onSubmit)}>
      <h1 className="font-semibold">Create new app</h1>
      <div className="mb-5 mt-3">
        <label
          htmlFor="name"
          className="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
        >
          Name
        </label>
        <input
          type="text"
          id="name"
          {...register("name")}
          className="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
        {errors.name && <p className="text-red-500">{errors.name.message}</p>}
      </div>
      <div className="flex">
        <Button
          size="sm"
          className="me-1 ms-auto"
          color="light"
          onClick={() => navigate("/apps")}
        >
          Cancel
        </Button>
        <Button size="sm" type="submit">
          Save
        </Button>
      </div>
    </form>
  );
}
export default AppCreate;
