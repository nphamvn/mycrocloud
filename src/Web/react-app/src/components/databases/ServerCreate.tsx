import { useAuth0 } from "@auth0/auth0-react";
import { useForm } from "react-hook-form";

type FormData = {
  name: string;
  //description?: string;
  //url: string;
  loginId: string;
  password: string;
  //port: number;
  //database: string;
  //type: string;
};
export default function ServerCreate() {
  const { register, handleSubmit } = useForm<FormData>();
  const { getAccessTokenSilently } = useAuth0();
  const onSubmit = async (data: FormData) => {
    const accessToken = await getAccessTokenSilently();
    const res = await fetch("/api/databaseServers", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${accessToken}`,
        "content-type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (res.ok) {
      console.log("success");
    }
  };
  return (
    <div>
      <h1>Server Create</h1>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div>
          <label htmlFor="name">Name</label>
          <input
            type="text"
            id="name"
            {...register("name")}
            className="block w-full border border-gray-300 bg-gray-50 p-1.5  text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
          />
        </div>
        <div className="mt-1">
          <label htmlFor="loginId">LoginId</label>
          <input
            type="text"
            id="loginId"
            {...register("loginId")}
            className="block w-full border border-gray-300 bg-gray-50 p-1.5  text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
          />
        </div>
        <div className="mt-1">
          <label htmlFor="password">Password</label>
          <input
            type="password"
            id="password"
            {...register("password")}
            className="block w-full border border-gray-300 bg-gray-50 p-1.5  text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
          />
        </div>
        <div className="mt-1">
          <button type="submit">Create</button>
        </div>
      </form>
    </div>
  );
}
