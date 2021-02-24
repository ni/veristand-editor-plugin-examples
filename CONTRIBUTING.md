# Contributing to VeriStand Editor Plugin Examples

Contributions to veristand-editor-plugin-examples are welcome from all!

The veristand-editor-plugin-examples repo is managed via [git](https://git-scm.com). The canonical upstream
repository is hosted on [GitHub](https://github.com/ni/veristand-editor-plugin-examples/).

This project follows a pull-request model for development. To contribute to the repo, you must:
1. Create a GitHub account.
1. Fork this project.
1. Sign off on your commits, such as by using `git commit -s` in the command line client. This will amend your git commit message with a line in the following format: `Signed-off-by: Name Lastname <name.lastmail@emailaddress.com>`.
1. Include all authors of any given commit into the commit message with a `Signed-off-by` line. This line indicates that you have read and signed the Developer Certificate of Origin (see below) and are able to legally submit your code to this repository.
1. Push a branch with your changes to the project.
1. Submit a pull request.

For more information, refer to [GitHub's official documentation](https://help.github.com/articles/using-pull-requests/).

# Getting Started

Ensure the solution builds for your version of VeriStand. To add code that calls into new APIs, use a constant defined in the project and *#if* statements to allow previous versions to build.

# Testing

The examples are manually tested. Changes to the *CustomApplicationFeatureSet* require running *VeriStand.CustomApplication* to verify.

# Developer Certificate of Origin (DCO)

   Developer's Certificate of Origin 1.1

   By making a contribution to this project, I certify that:

   (a) The contribution was created in whole or in part by me and I
       have the right to submit it under the open source license
       indicated in the file; or

   (b) The contribution is based upon previous work that, to the best
       of my knowledge, is covered under an appropriate open source
       license and I have the right under that license to submit that
       work with modifications, whether created in whole or in part
       by me, under the same open source license (unless I am
       permitted to submit under a different license), as indicated
       in the file; or

   (c) The contribution was provided directly to me by some other
       person who certified (a), (b) or (c) and I have not modified
       it.

   (d) I understand and agree that this project and the contribution
       are public and that a record of the contribution (including all
       personal information I submit with it, including my sign-off) is
       maintained indefinitely and may be redistributed consistent with
       this project or the open source license(s) involved.

(taken from [developercertificate.org](https://developercertificate.org/))

For more information on how veristand-editor-plugin-examples is licensed, refer to the [LICENSE](https://github.com/ni/veristand-editor-plugin-examples/blob/master/LICENSE).
