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