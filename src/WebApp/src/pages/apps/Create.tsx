import { SubmitHandler, useForm } from "react-hook-form";
import { toast } from "react-toastify";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import { useNavigate } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";

type Inputs = {
  name: string;
  description?: string;
};

function AppCreate() {
  const navigate = useNavigate();
  const { getAccessTokenSilently } = useAuth0();
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
      const accessToken = await getAccessTokenSilently();
      const res = await fetch("/api/apps", {
        method: "POST",
        headers: {
          "content-type": "application/json",
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify(data),
      });
      if (res.ok) {
        toast("Created app");
        navigate("/apps");
      }
    } catch (error) {
      console.log(error);
    }
  };
  return (
    <form className="mx-auto mt-5 max-w-4xl" onSubmit={handleSubmit(onSubmit)}>
      <h1 className="font-semibold">Create app</h1>
      <div className="mb-5 mt-3">
        <label
          htmlFor="name"
          className="mb-2 block  font-medium text-gray-900 dark:text-white"
        >
          Name
        </label>
        <input
          type="text"
          id="name"
          {...register("name")}
          className="block w-full border border-gray-300 bg-gray-50 p-1.5  text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
        {errors.name && <p className="text-red-500">{errors.name.message}</p>}
      </div>
      <div className="mb-5 mt-3">
        <label
          htmlFor="description"
          className="mb-2 block  font-medium text-gray-900 dark:text-white"
        >
          Description
        </label>
        <textarea
          id="description"
          rows={4}
          {...register("description")}
          className="block w-full border border-gray-300 bg-gray-50  text-gray-900 focus:border-cyan-500 focus:ring-cyan-500 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-cyan-500 dark:focus:ring-cyan-500"
        ></textarea>
        {errors.description && (
          <p className="text-red-500">{errors.description.message}</p>
        )}
      </div>
      <div className="flex">
        <button
          type="button"
          className="group relative me-1 ms-auto flex items-center justify-center border border-gray-300 bg-white p-0.5 text-center font-medium text-gray-900 focus:z-10 focus:outline-none focus:ring-2 focus:ring-cyan-300 enabled:hover:bg-gray-100 dark:border-gray-600 dark:bg-gray-600 dark:text-white dark:focus:ring-gray-700 dark:enabled:hover:border-gray-700 dark:enabled:hover:bg-gray-700"
          onClick={() => navigate("/apps")}
        >
          <span className="flex items-center rounded-md px-3 py-1.5  transition-all duration-200">
            Cancel
          </span>
        </button>
        <button
          type="submit"
          className="group relative flex items-center justify-center border border-transparent bg-cyan-700 p-0.5 text-center font-medium text-white focus:z-10 focus:outline-none focus:ring-2 focus:ring-cyan-300 enabled:hover:bg-cyan-800 dark:bg-cyan-600 dark:focus:ring-cyan-800 dark:enabled:hover:bg-cyan-700"
        >
          <span className="flex items-center rounded-md px-3 py-1.5  transition-all duration-200">
            Save
          </span>
        </button>
      </div>
    </form>
  );
}
export default AppCreate;
