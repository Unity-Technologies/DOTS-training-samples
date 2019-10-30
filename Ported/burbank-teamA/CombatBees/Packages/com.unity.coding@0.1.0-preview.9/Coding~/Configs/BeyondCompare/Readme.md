# About Unity's Beyond Compare Configuration #

This config contains two new file formats:

  * `[Unity] C-Style Source`
  * `[Unity] Perl Scripts`

The most important things they set are "Insert spaces instead of tabs" and
"When saving -> Trim trailing whitespace".

There's also a little goodie in the C-Style grammar `[Unity] Format Whitespace`
which can be selected using Session Settings into the current/parent/default
session as "unimportant", enabling ignoring boring whitespace-only format stuff
from diffs.

# Format Installation #

Just double-click the .bcpkg file to import it. Alternately, you can use
"Tools | Import Settings..." and select the .bcpkg file.

Importing the config _will not overwrite_ installed BC file formats. Instead,
it will add the two above formats at top so they will override. You can easily
inspect them after import to see how they are configured.

# Usage/Installation of (Un)Importance Rule #

To ignore unimportant whitespace during a diff:

  1. Session | Session Settings... -> Importance
  2. Uncheck `[Unity] Format Whitespace` so it is unmarked as important
  3. Uncheck `Leading whitespace` and `Trailing whitespace`
  3. (Optional) Use the dropdown at bottom to widen scope of this setting
  4. OK

Now you can use the "Ignore Unimportant Differences" button (also on the View
menu) to hide noisy format-related whitespace stuff from your diff.

*Important*: keep this option off by default, and if you use it, turn it back
off as soon as you're done. It's very easy to have accidental "unimportant"
changes get into a commit, which noises up PR's for no reason and annoys
reviewers who don't appreciate the extra work. You won't notice these diffs in
advance if you have this option on.
