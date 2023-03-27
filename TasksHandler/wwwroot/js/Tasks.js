function addNewTask() {
    TasksListDTO.tasks.push(new TasksElementListDTO({ id: 0, Title: '' }));
    $("[name=Title-task]").last().focus();
}

async function focusoutTaskTitle(task) {
    const Title = task.title();
    if (!Title) {
        TasksListDTO.tasks.push(new TasksElementListDTO({ id: 0, title: '' }));
        $("[name=title-task]").last().focus();
    }
}

async function focusoutTaskTitle(task) {
    const title = task.Title();
    if (!title) {
        TasksListDTO.tasks.pop();
        return;
    }
    //task.id(1);
    const data = JSON.stringify(title);
    const response = await fetch(urlTasks, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (response.ok) {
        const json = await response.json();
        task.id(json.id);
    } else {
        HandleErrorApi(response);
    }
}

async function getTasks() {
    TasksListDTO.loading(true);

    const response = await fetch(urlTasks, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })

    if (!response.ok) {
        HandleErrorApi(response);
        return;
    }

    const json = await response.json();
    TasksListDTO.tasks([]);

    json.forEach(valor => {
        TasksListDTO.tasks.push(new TasksElementListDTO(valor));
    });

    TasksListDTO.loading(false);
}

async function updateTasksOrder() {
    const ids = getTasksIds();
    await sendTasksIdsToBackend(ids);

    const sortedArray = TasksListDTO.tasks.sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id.toString());
    });

    TasksListDTO.tasks([]);
    getTasks();
}

function getTasksIds() {
    const ids = $("[name=Title-task]").map(function () {
        return $(this).attr("data-id");
    }).get();

    return ids;
}

async function sendTasksIdsToBackend(ids) {
    var data = JSON.stringify(ids);
    await fetch(`${urlTasks}/sort`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

async function TaskClickHandler(task) {
    if (task.isNew()) {
        return;
    }

    const response = await fetch(`${urlTasks}/${task.id()}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        HandleErrorApi(response);
        return;
    }

    const json = await response.json();
    console.log(json);

    TaskEditVM.id = json.id;
    TaskEditVM.title(json.title);
    TaskEditVM.description(json.description);

    TaskEditVM.steps([]);

    json.steps.forEach(step => {
        TaskEditVM.steps.push(
            new StepViewModel({ ...step, editMode: false })
            )
    })

    EditTaskModalBootstrap.show();
}


async function EditChangeTaskHandler() {
    const obj = {
        id: TaskEditVM.id,
        title: TaskEditVM.title(),
        description: TaskEditVM.description()
    };

    if (!obj.title) {
        return;
    }

    await editCompleteTask(obj);

    const indice = TasksListDTO.tasks().findIndex(t => t.id() === obj.id);
    const task = TasksListDTO.tasks()[indice];
    task.title = obj.title;
    getTasks();
}

function DeleteTaskChoose(task) {
    EditTaskModalBootstrap.hide();

    confirmAction({
        callBackAccept: () => {
            deleteTask(task);
        },
        callBackCancel: () => {
            EditTaskModalBootstrap.show();
        },
        title: `Do you want to delete the task ${task.title()}?`
        })
}

async function deleteTask(task) {
    const idTask = task.id;

    const response = await fetch(`${urlTasks}/${idTask}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (response.ok) {
        const indice = getEditTaskIndex();
        TasksListDTO.tasks.splice(indice, 1);
    }
}

function getEditTaskIndex() {
    return TasksListDTO.tasks().findIndex(t => t.id() == TaskEditVM.id);
}

async function editCompleteTask(task) {
    const data = JSON.stringify(task);

    const response = await fetch(`${urlTasks}/${task.id}`, {
        method: 'PUT',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        HandleErrorApi(response);
        throw "error";
    }
}

$(function () {
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await updateTasksOrder();
        }
    })
})