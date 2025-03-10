# File Diff

## Usage
To run the tool open your command line on the folder containing the executable and pass in the two paths of the files you want to diff as arguments to the call
### Example
```
.\filediff.exe .\filediff1.cpp .\filediff2.cpp
```

### Sample Output
![SampleOutput](https://github.com/user-attachments/assets/af350f8e-422e-42ce-af58-dec401d9be97)
![image](https://github.com/user-attachments/assets/8387e77f-884c-4510-890f-e0a7d946fce4)

### Test files
Test files can be found under `difftool.Tests/testFiles` in this same repo.

## Troubleshooting
This app requires .NET runtime v6. Verify that this is installed in your machine and in the case it isn't you can download it through this link: https://aka.ms/dotnet-core-applaunch?missing_runtime=true&arch=x64&rid=win10-x64&apphost_version=6.0.22

## Stack for developing the console app
Project requires the following to build
- Visual Studio 2022
- .NET 6

## Implementation
This application implements the dynamic programming solution for the longest common sequences. A matrix with the lengnths of the possible longes common sequences is built. Given this, we can determine between which lines things have been inserted or deleted. If a change of the orientation of iteration of the backtrack happens we save the index where it happened and once the iteration orientation ends or we run out of indices to go through we assing the index where this happen as the end of the block. Since we are backtracking we reverse the list of index pairs. After that we iterate through the lists of pairs in order to print the diff. The code contains comments that go into more depth of implementations.

### Performance
This dynamic programming solution has both runtime and space wise complexity of `O(n x m)`, `n` being the length of the first list of strings and the `m` being the length of the second list of strings.

Another possible overhead on this approach is the comparison time since this can be time consuming given long strings. Extra work can be performed to mitigate this by hashing the strings and compare the hashes instead.

Reading the files lines can also be time consuming. A reading stream can be used instead of fetching all the file lines at once. This would be specially noticeable for large files.

## Tests
Automated tests were added to the solution file in order maintine the outputs validity after refactors and code clean up.
The main testing performed is a function that reads the output of the program and validates that when following the instructions of the output the base file will be the same as the target file.
