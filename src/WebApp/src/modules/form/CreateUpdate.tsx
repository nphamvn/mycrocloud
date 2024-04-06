import { useAuth0 } from "@auth0/auth0-react";
import { useEffect } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { Link, useNavigate, useParams } from "react-router-dom";
import { toast } from "react-toastify";
import { v4 as uuidv4 } from "uuid";
import IForm, { IFormField } from "./IForm";

type Inputs = { name: string; description: string; fields: FieldInputs[] };
type FieldInputs = {
  id: string;
  name: string;
  type: "TextInput" | "NumberInput";
  details: Details;
};
type Details = {
  textInput?: TextInput | null;
  numberInput?: NumberInput | null;
};
type TextInput = {
  minLength?: number;
  maxLength?: number;
};
type NumberInput = {
  min?: number;
  max?: number;
};

const sampleInputs: Inputs = {
  name: "User Form",
  description: "Form for user data",
  fields: [
    {
      id: uuidv4(),
      name: "Name",
      type: "TextInput",
      details: {
        textInput: {
          minLength: 3,
          maxLength: 50,
        },
      },
    },
    {
      id: uuidv4(),
      name: "Age",
      type: "NumberInput",
      details: {
        numberInput: {
          min: 18,
          max: 100,
        },
      },
    },
  ],
};
export default function CreateUpdate() {
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
  const id = useParams()["id"] ? parseInt(useParams()["id"]!) : null;

  const {
    control,
    register,
    formState: { errors },
    handleSubmit,
    watch,
    setValue,
  } = useForm<Inputs>({
    //defaultValues: sampleInputs,
  });
  //https://react-hook-form.com/docs/usefieldarray#Controlled Field Array

  const { fields, append } = useFieldArray({
    control,
    name: "fields",
  });

  const watchFields = watch("fields");
  const controlledFields = fields.map((field, index) => {
    return {
      ...field,
      ...watchFields[index],
    };
  });

  useEffect(() => {
    if (id) {
      const getForm = async () => {
        const accessToken = await getAccessTokenSilently();
        const response = await fetch(`/formapi/forms/${id}`, {
          headers: { Authorization: `Bearer ${accessToken}` },
        });
        const data = (await response.json()) as IForm;
        setValue("name", data.name);
        setValue("description", data.description);
        setValue(
          "fields",
          data.fields.map((field) => reverseMapFormField(field)),
        );
      };

      function reverseMapFormField(field: IFormField): FieldInputs {
        return {
          id: field.id,
          name: field.name,
          type: field.type,
          details: field.details, //TODO: reverse map details
        };
      }
      getForm();
    }
  }, []);

  const onSubmit = async (data: Inputs) => {
    console.log(data);
    try {
      const accessToken = await getAccessTokenSilently();
      const url = id ? `/formapi/forms/${id}` : "/formapi/forms";
      const method = id ? "PUT" : "POST";
      const response = await fetch(url, {
        method: method,
        headers: {
          Authorization: `Bearer ${accessToken}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      });
      if (response.ok) {
        if (id) {
          toast.success("Form saved successfully");
          return;
        }
        navigate(`../${parseInt(response.headers.get("Location")!)}`);
        return;
      }
      throw new Error("Failed to save form");
    } catch (error) {
      toast.error("Something went wrong");
    }
  };

  const onInvalid = (errors: any) => {
    console.log(errors);
  };

  return (
    <>
      <Link to="..">Back</Link>
      <h1 className="font-bold">{id ? "Edit Form" : "New Form"}</h1>
      {id && <Link to="submissions">Submissions</Link>}
      <form onSubmit={handleSubmit(onSubmit, onInvalid)} className="mt-3">
        <div>
          <label htmlFor="name">Name</label>
          <input
            type="text"
            {...register("name")}
            id="name"
            className="mt-1 block w-full border px-2 py-1"
          />
        </div>

        <div className="mt-2">
          <label htmlFor="description">Description</label>
          <textarea
            {...register("description")}
            id="description"
            className="mt-1 block w-full border px-2 py-1"
          />
        </div>

        <section>
          <h2 className="mt-2 font-semibold">Fields</h2>
          {controlledFields.map((field, index) => (
            <div key={field.id}>
              <div>
                <label htmlFor={`fields.${index}.name`}>Name</label>
                <input
                  type="text"
                  {...register(`fields.${index}.name` as const)}
                  id={`fields.${index}.name`}
                  className="mt-1 block w-full border px-2 py-1"
                />
              </div>
              <div className="mt-2">
                <label htmlFor={`fields.${index}.type`}>Type</label>
                <select
                  {...register(`fields.${index}.type` as const)}
                  id={`fields.${index}.type`}
                  className="mt-1 block"
                >
                  <option value="TextInput">Text Input</option>
                  <option value="NumberInput">Number Input</option>
                </select>
              </div>
              <div className="mt-2">
                {field.type === "TextInput" && (
                  <div className="flex space-x-2">
                    <div>
                      <label
                        htmlFor={`fields.${index}.details.textInput.minLength`}
                      >
                        Min Length
                      </label>
                      <input
                        type="number"
                        {...register(
                          `fields.${index}.details.textInput.minLength` as const,
                        )}
                        id={`fields.${index}.details.textInput.minLength`}
                        className="mt-1 block border px-2 py-1"
                      />
                    </div>
                    <div>
                      <label
                        htmlFor={`fields.${index}.details.textInput.maxLength`}
                      >
                        Max Length
                      </label>
                      <input
                        type="number"
                        {...register(
                          `fields.${index}.details.textInput.maxLength` as const,
                        )}
                        id={`fields.${index}.details.textInput.maxLength`}
                        className="mt-1 block border px-2 py-1"
                      />
                    </div>
                  </div>
                )}
                {field.type === "NumberInput" && (
                  <div className="flex space-x-2">
                    <div>
                      <label
                        htmlFor={`fields.${index}.details.numberInput.min`}
                      >
                        Min
                      </label>
                      <input
                        type="number"
                        {...register(
                          `fields.${index}.details.numberInput.min` as const,
                        )}
                        id={`fields.${index}.details.numberInput.min`}
                        className="mt-1 block border px-2 py-1"
                      />
                    </div>
                    <div>
                      <label
                        htmlFor={`fields.${index}.details.numberInput.max`}
                      >
                        Max
                      </label>
                      <input
                        type="number"
                        {...register(
                          `fields.${index}.details.numberInput.max` as const,
                        )}
                        id={`fields.${index}.details.numberInput.max`}
                        className="mt-1 block border px-2 py-1"
                      />
                    </div>
                  </div>
                )}
              </div>
            </div>
          ))}
          <button
            type="button"
            onClick={() => {
              append({
                id: uuidv4(),
                name: "",
                type: "TextInput",
                details: {},
              });
            }}
            className="mt-2 rounded bg-secondary px-3 py-1 text-white"
          >
            Add
          </button>
        </section>

        <button type="submit" className="mt-3 bg-primary px-3 py-1 text-white">
          Save
        </button>
      </form>
    </>
  );
}
