# MpiWorker
This project helps you multiply matrices. Matrices must be of size NxN, and each call consists of ```i + l + 1```

## Run
Example of running this program:
```
mpiexec.exe -n [count of workers] .\bin\Debug\net5.0\MpiWorker.exe -n [N] -p
```
* ```-n [count of workers]``` - Set the count of processes that could work;
* ```-n [N]``` - Set the number of columns and rows in the matrix;
* ```-p``` - Allow print debug messages to the console ```(optional)```.

> [!WARNING]
> ```Count of workers``` must be above then 2.

## Work
Workers are divided into 2 types:
1. ```Master``` - Worker with rank 0
    - Distributes work
    - Merge matrix
2. ```Slave``` - Another workers
    - Calculate matrix

## Example
```
mpiexec.exe -n 2 .\bin\Debug\net5.0\MpiWorker.exe -n 3 -p
```
![MpiWorkerExample](https://github.com/user-attachments/assets/ca826e28-f177-439e-b38a-86bf51f34f00)

```
mpiexec.exe -n 3 .\bin\Debug\net5.0\MpiWorker.exe -n 5 -p
```
![MpiWorkerExample](https://github.com/user-attachments/assets/b964a1bc-519f-4fd3-9290-e21d750181db)

```
mpiexec.exe -n 8 .\bin\Debug\net5.0\MpiWorker.exe -n 7
```
![MpiWorkerExample](https://github.com/user-attachments/assets/ab896332-0a1b-4572-8c12-1029c40065da)
