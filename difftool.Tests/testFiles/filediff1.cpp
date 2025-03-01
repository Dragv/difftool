// filediff1.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>



int main(int argc, const char *argv[])
{
    const char* filename1 = nullptr;
    const char* filename2 = nullptr;
    for (int i = 1; i < argc; ++i)
    {
        if (filename1 != nullptr)
        {
            filename2 = argv[i];


        }
        else
            filename1 = argv[i];
    }
    if (filename2 == nullptr)
    {
        std::cout << "Usage: filediff <filename1> <filename2>\n";
        std::cout << "Produces diff steps to create filename2 from filename1\n";
        return -1;
    }
    std::cout << "Comparing" << filename1 << " with " << filename2 << "\n";
    return 0;
}
//*
