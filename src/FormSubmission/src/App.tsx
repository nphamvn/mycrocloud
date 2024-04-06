import { useEffect, useState } from "react";
import "./App.css";
import IForm from "./IForm";
import { useFieldArray, useForm } from "react-hook-form";

type FieldValue = {
  fieldId: string;
  value: any;
};

type Data = {
  values: FieldValue[];
};

function buildSchema(form: IForm) {}

function App() {
  const queryParameters = new URLSearchParams(window.location.search);
  const id = queryParameters.get("id");
  const [form, setForm] = useState<IForm>();

  const { control, register, setValue, handleSubmit } = useForm<Data>({
    //resolver:
  });

  const { fields } = useFieldArray({
    control,
    name: "values",
  });

  const onSubmit = async (data: Data) => {
    console.log(data);
    const response = await fetch(`/formapi/forms/${id}/submissions`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data.values),
    });
    if (response.ok) {
      alert("Form submitted successfully");
    } else {
      alert("Form submission failed");
    }
  };

  useEffect(() => {
    const fetchForm = async () => {
      const response = await fetch(`/formapi/forms/${id}/public`);
      const data = (await response.json()) as IForm;
      setForm(data);
      setValue(
        "values",
        data.fields.map((field) => ({ fieldId: field.id, value: undefined }))
      );
    };
    fetchForm();
  }, []);

  if (!id) {
    return <div>Not Found</div>;
  }

  if (!form) {
    return <div>Loading...</div>;
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <h1 className="font-bold">{form.name}</h1>
      <p>{form.description}</p>
      <section className="mt-3 flex flex-col space-y-2">
        {fields.map((field, index) => {
          const f = form.fields.find((f) => f.id === field.fieldId)!;
          return (
            <div key={field.fieldId}>
              <div>
                <label htmlFor={`values.${index}.value`} className="mb-1">{f.name}</label>
                {f.type === "TextInput" && (
                  <input
                    type="text"
                    {...register(`values.${index}.value` as const)}
                    id={`values.${index}.value`}
                    className="border w-full px-2 py-1"
                    aria-autocomplete="none"
                  />
                )}
                {f.type === "NumberInput" && (
                  <input
                    type="number"
                    {...register(`values.${index}.value` as const, {
                      valueAsNumber: true,
                    })}
                    id={`values.${index}.value`}
                    className="border w-full px-2 py-1"
                  />
                )}
              </div>
            </div>
          );
        })}
      </section>
      <button type="submit" className="bg-primary px-3 py-1 text-white mt-2">Submit</button>
    </form>
  );
}

export default App;
